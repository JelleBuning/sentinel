using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.Disks.v1;

public sealed class UpdateDisksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/devices/{id}/disks", async ([FromRoute] int id, [FromBody] List<DiskDto> body, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new UpdateDisksCommand(id, body), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("UpdateDisks")
            .WithTags("Devices")
            .RequireAuthorization("Device");
    }
}
