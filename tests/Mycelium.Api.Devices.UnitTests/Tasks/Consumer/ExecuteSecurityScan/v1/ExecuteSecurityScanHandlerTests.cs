using Mycelium.Api.Devices.SignalR;
using Mycelium.Api.Devices.Tasks.Consumer.ExecuteSecurityScan.v1;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Tasks.Consumer.ExecuteSecurityScan.v1;

public class ExecuteSecurityScanHandlerTests
{
    [Test]
    public async Task Handle_SendsSecurityScanRequestForDevice()
    {
        var messenger = Substitute.For<IDeviceMessenger>();
        var handler = new ExecuteSecurityScanHandler(messenger);

        var result = await handler.Handle(new ExecuteSecurityScanCommand(5), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        await messenger.Received(1).SendSecurityScanRequestAsync(5, Arg.Any<CancellationToken>());
    }
}
