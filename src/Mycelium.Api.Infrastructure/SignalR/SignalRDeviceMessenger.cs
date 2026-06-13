using Microsoft.AspNetCore.SignalR;
using Mycelium.Api.Application.Interfaces;
using Mycelium.Api.Infrastructure.SignalR.Interfaces;
using Mycelium.Common.SignalR;

namespace Mycelium.Api.Infrastructure.SignalR;

public class SignalRDeviceMessenger(IHubContext<DeviceMessageHub, IDeviceMessageHub> hubContext)
    : IDeviceMessenger
{
    public async Task SendSecurityScanRequestAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        // TODO: Replace UserHandler.ConnectedIds.First() with proper device connection lookup by deviceId
        var connectionId = UserHandler.ConnectedIds.First();
        var client = hubContext.Clients.Client(connectionId);
        await client.SecurityScanMessage(new SecurityScanMessage());
    }

    public async Task SendRestartRequestAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        // TODO: Replace UserHandler.ConnectedIds.First() with proper device connection lookup by deviceId
        var connectionId = UserHandler.ConnectedIds.First();
        var client = hubContext.Clients.Client(connectionId);
        await client.RestartDeviceMessage(new RestartDeviceMessage());
    }

    public async Task SendRemoteAccessRequestAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        // TODO: Replace UserHandler.ConnectedIds.First() with proper device connection lookup by deviceId
        var connectionId = UserHandler.ConnectedIds.First();
        var client = hubContext.Clients.Client(connectionId);
        await client.RemoteAccessMessage(new RemoteAccessMessage());
    }
}
