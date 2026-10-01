using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Get.SoftwareInformation.v1;

public sealed class GetSoftwareInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices/{id}/software", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetSoftwareInformationQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("GetSoftwareInformation")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
