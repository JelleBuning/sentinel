using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Get.ById.v1;

public sealed class GetDeviceInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices/{id}", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetDeviceInformationQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("GetDeviceInformation")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
