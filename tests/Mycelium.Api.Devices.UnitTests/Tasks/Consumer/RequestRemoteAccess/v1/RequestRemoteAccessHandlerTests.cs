using Mycelium.Api.Devices.SignalR;
using Mycelium.Api.Devices.Tasks.Consumer.RequestRemoteAccess.v1;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Tasks.Consumer.RequestRemoteAccess.v1;

public class RequestRemoteAccessHandlerTests
{
    [Test]
    public async Task Handle_SendsRemoteAccessRequestForDevice()
    {
        var messenger = Substitute.For<IDeviceMessenger>();
        var handler = new RequestRemoteAccessHandler(messenger);

        var result = await handler.Handle(new RequestRemoteAccessCommand(7), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        await messenger.Received(1).SendRemoteAccessRequestAsync(7, Arg.Any<CancellationToken>());
    }
}
