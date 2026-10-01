using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.SecurityInformation.v1;

public sealed class UpdateSecurityInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/devices/{id}/security", async ([FromRoute] int id, [FromBody] SecurityDto body, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new UpdateSecurityInformationCommand(id, body), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("UpdateSecurityInformation")
            .WithTags("Devices")
            .RequireAuthorization("Device");
    }
}
