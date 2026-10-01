using Microsoft.AspNetCore.Builder;
using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;

namespace Mycelium.Api.Devices.Get.All.v1;

public sealed class GetAllDevicesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices", async (ClaimsPrincipal user, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetAllDevicesQuery(user.GetId()), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("GetAllDevices")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
