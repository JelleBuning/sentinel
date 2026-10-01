using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Tasks.Scheduled.Ping.v1;

public sealed class PingDeviceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/devices/{id}/ping", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new PingDeviceCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("PingDevice")
            .WithTags("Devices")
            .RequireAuthorization("Device");
    }
}
