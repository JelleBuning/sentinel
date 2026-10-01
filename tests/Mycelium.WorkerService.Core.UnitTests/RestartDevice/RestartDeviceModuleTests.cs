using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.SignalR;
using Mycelium.WorkerService.Core.RestartDevice;
using Mycelium.WorkerService.Core.UnitTests.TestSupport;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.RestartDevice;

public class RestartDeviceModuleTests
{
    [Test]
    public async Task OnMessageReceived_ReturnsTrue()
    {
        var config = ConsumerConfigFactory.Create<RestartDeviceMessage>();
        var module = new RestartDeviceModule(config, NullLogger<RestartDeviceMessage>.Instance);

        var resultTask = (Task<bool>)ConsumerConfigFactory.InvokeOnMessageReceived(module, new RestartDeviceMessage());

        Assert.That(await resultTask, Is.True);
    }
}
