using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Get.Disks.v1;

public sealed class GetDisksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices/{id}/disks", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetDisksQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("GetDisks")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
