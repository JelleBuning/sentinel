using System.Security.Claims;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Tasks.Startup.Register.v1;

public sealed class RegisterDeviceHandler(AppDbContext dbContext, ITokenGenerator tokenGenerator)
    : ICommandHandler<RegisterDeviceCommand, Result<DeviceTokenResponse>>
{
    public async ValueTask<Result<DeviceTokenResponse>> Handle(RegisterDeviceCommand command, CancellationToken cancellationToken)
    {
        var organisation = await dbContext.Organisations
            .FirstOrDefaultAsync(o => o.Hash == command.OrganisationHash, cancellationToken);

        if (organisation is null)
        {
            return Result.Failure<DeviceTokenResponse>(Error.NotFound("Organisation not found"));
        }

        var device = new Device
        {
            Name = command.Name,
            CreatedOn = DateTime.Now,
            LastActive = DateTime.Now,
            RefreshToken = tokenGenerator.GenerateRefreshToken(),
        };
        organisation.Devices.Add(device);
        await dbContext.SaveChangesAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new("Id", device.Id.ToString()),
            new("Name", device.Name),
            new(ClaimTypes.Role, "Device"),
        };

        return Result.Success(new DeviceTokenResponse
        {
            Id = device.Id,
            OrganisationId = organisation.Id,
            AccessToken = tokenGenerator.GenerateAccessToken(claims),
            RefreshToken = device.RefreshToken,
        });
    }
}
