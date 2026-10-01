using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Tasks.Consumer.ExecuteSecurityScan.v1;

public sealed class ExecuteSecurityScanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/devices/securityScan/{id}", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new ExecuteSecurityScanCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("ExecuteSecurityScan")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
