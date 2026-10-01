using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Tasks.Startup.Register.v1;

public sealed class RegisterDeviceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/devices/register", async ([FromBody] RegisterDeviceRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = new RegisterDeviceCommand(request.OrganisationHash, request.Name);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("RegisterDevice")
            .WithTags("Devices")
            .AllowAnonymous();
    }
}
