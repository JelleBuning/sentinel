using Microsoft.AspNetCore.SignalR;

namespace Sentinel.Api.Extensions;

public static class SignalRExtensions
{
    public static HubEndpointConventionBuilder AddHub<THub>(this IEndpointRouteBuilder endpoints) where THub : Hub
    {
        var pattern = typeof(THub).Name;
        return endpoints.MapHub<THub>(pattern, configureOptions: null);
    }
}