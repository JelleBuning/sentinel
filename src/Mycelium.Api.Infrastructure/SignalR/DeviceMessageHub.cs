using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mycelium.Api.Infrastructure.SignalR.Interfaces;

namespace Mycelium.Api.Infrastructure.SignalR;

[Authorize(Roles = "User,Device")]
public class DeviceMessageHub : Hub<IDeviceMessageHub>
{
    public override Task OnConnectedAsync()
    {
        UserHandler.ConnectedIds.Add(Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        UserHandler.ConnectedIds.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}

public static class UserHandler
{
    public static readonly HashSet<string> ConnectedIds = [];
}