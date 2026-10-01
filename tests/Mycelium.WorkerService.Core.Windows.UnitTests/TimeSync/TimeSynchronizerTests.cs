using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.Core.Windows.TimeSync;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.TimeSync;

public class TimeSynchronizerTests
{
    [Test]
    public async Task Synchronize_StartsPowerShellWithSyncScriptAndAwaitsExit()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var processHandle = Substitute.For<IProcessHandle>();
        processHandle.WaitForExitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        processRunner.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(processHandle);

        var synchronizer = new TimeSynchronizer(processRunner);

        await synchronizer.Synchronize();

        processRunner.Received(1).Start(
            "C:\\windows\\system32\\windowspowershell\\v1.0\\powershell.exe",
            Arg.Is<string>(script => script.Contains("w32tm /resync /force") && script.Contains("Set-Service w32time -StartupType manual")));
        await processHandle.Received(1).WaitForExitAsync(Arg.Any<CancellationToken>());
    }
}
