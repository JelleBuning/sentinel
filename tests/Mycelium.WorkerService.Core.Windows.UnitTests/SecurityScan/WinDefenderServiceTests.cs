using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.Core.Windows.SecurityScan;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.SecurityScan;

public class WinDefenderServiceTests
{
    private static IProcessHandle BuildHandle(int exitCode)
    {
        var handle = Substitute.For<IProcessHandle>();
        handle.ExitCode.Returns(exitCode);
        handle.WaitForExitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        return handle;
    }

    [Test]
    public async Task Scan_DefenderUnavailable_ReturnsFalseWithoutStartingProcess()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var service = new WinDefenderService(processRunner, "C:\\fake\\MpCmdRun.exe", isDefenderAvailable: false);

        var result = await service.Scan(CancellationToken.None);

        Assert.That(result, Is.False);
        processRunner.DidNotReceive().Start(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Scan_ExitCodeTwo_ReturnsTrue()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var handle = BuildHandle(2);
        processRunner.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(handle);
        var service = new WinDefenderService(processRunner, "C:\\fake\\MpCmdRun.exe", isDefenderAvailable: true);

        var result = await service.Scan(CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Scan_NonMatchingExitCode_ReturnsFalse()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var handle = BuildHandle(0);
        processRunner.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(handle);
        var service = new WinDefenderService(processRunner, "C:\\fake\\MpCmdRun.exe", isDefenderAvailable: true);

        var result = await service.Scan(CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Scan_StartsProcessWithQuickScanArguments()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var handle = BuildHandle(2);
        processRunner.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(handle);
        var service = new WinDefenderService(processRunner, "C:\\fake\\MpCmdRun.exe", isDefenderAvailable: true);

        await service.Scan(CancellationToken.None);

        processRunner.Received(1).Start("C:\\fake\\MpCmdRun.exe", Arg.Is<string>(args => args.Contains("-Scan") && args.Contains("-ScanType")));
    }
}
