using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Tasks.Consumer.RequestRemoteAccess.v1;

public sealed class RequestRemoteAccessEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/devices/remoteAccess/{id}", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new RequestRemoteAccessCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("RequestRemoteAccess")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
