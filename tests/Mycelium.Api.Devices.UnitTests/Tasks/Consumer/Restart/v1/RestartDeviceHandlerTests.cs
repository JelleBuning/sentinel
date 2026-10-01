using Mycelium.Api.Devices.SignalR;
using Mycelium.Api.Devices.Tasks.Consumer.Restart.v1;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Tasks.Consumer.Restart.v1;

public class RestartDeviceHandlerTests
{
    [Test]
    public async Task Handle_SendsRestartRequestForDevice()
    {
        var messenger = Substitute.For<IDeviceMessenger>();
        var handler = new RestartDeviceHandler(messenger);

        var result = await handler.Handle(new RestartDeviceCommand(3), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        await messenger.Received(1).SendRestartRequestAsync(3, Arg.Any<CancellationToken>());
    }
}
