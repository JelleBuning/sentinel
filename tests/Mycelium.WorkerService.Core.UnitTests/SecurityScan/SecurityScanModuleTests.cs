using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.SignalR;
using Mycelium.WorkerService.Core.SecurityScan;
using Mycelium.WorkerService.Core.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.SecurityScan;

public class SecurityScanModuleTests
{
    [Test]
    public async Task OnMessageReceived_DelegatesToScannerAndReturnsItsResult()
    {
        var scanner = Substitute.For<ISecurityScanner>();
        scanner.Scan(Arg.Any<CancellationToken>()).Returns(true);
        var config = ConsumerConfigFactory.Create<SecurityScanMessage>();
        var module = new SecurityScanModule(config, NullLogger<SecurityScanMessage>.Instance, scanner);

        var resultTask = (Task<bool>)ConsumerConfigFactory.InvokeOnMessageReceived(module, new SecurityScanMessage());

        Assert.That(await resultTask, Is.True);
    }

    [Test]
    public void OnMessageReceived_ScannerThrows_ReturnsFaultedTask()
    {
        var scanner = Substitute.For<ISecurityScanner>();
        scanner.Scan(Arg.Any<CancellationToken>()).Returns(Task.FromException<bool>(new InvalidOperationException("boom")));
        var config = ConsumerConfigFactory.Create<SecurityScanMessage>();
        var module = new SecurityScanModule(config, NullLogger<SecurityScanMessage>.Instance, scanner);

        var resultTask = (Task<bool>)ConsumerConfigFactory.InvokeOnMessageReceived(module, new SecurityScanMessage());

        Assert.That(resultTask.IsFaulted, Is.True);
    }
}
