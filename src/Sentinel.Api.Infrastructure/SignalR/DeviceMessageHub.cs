using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Sentinel.Api.Infrastructure.SignalR.Interfaces;

namespace Sentinel.Api.Infrastructure.SignalR;

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