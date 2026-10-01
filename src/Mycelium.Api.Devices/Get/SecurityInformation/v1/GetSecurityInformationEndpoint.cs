using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Get.SecurityInformation.v1;

public sealed class GetSecurityInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices/{id}/security", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetSecurityInformationQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("GetSecurityInformation")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
