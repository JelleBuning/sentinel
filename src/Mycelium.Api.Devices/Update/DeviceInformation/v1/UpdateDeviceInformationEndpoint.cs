using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Devices.Update.DeviceInformation.v1;

public sealed class UpdateDeviceInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/devices/{id}", async ([FromRoute] int id, [FromBody] InformationDto body, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new UpdateDeviceInformationCommand(id, body), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("UpdateDeviceInformation")
            .WithTags("Devices")
            .RequireAuthorization("Device");
    }
}
