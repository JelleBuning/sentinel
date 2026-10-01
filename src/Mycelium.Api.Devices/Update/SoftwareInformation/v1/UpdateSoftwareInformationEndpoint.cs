using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.SoftwareInformation.v1;

public sealed class UpdateSoftwareInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/devices/{id}/software", async ([FromRoute] int id, [FromBody] List<SoftwareDto> body, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new UpdateSoftwareInformationCommand(id, body), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("UpdateSoftwareInformation")
            .WithTags("Devices")
            .RequireAuthorization("Device");
    }
}
