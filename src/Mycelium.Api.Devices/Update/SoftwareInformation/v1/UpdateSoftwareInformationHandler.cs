using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Devices.Update.SoftwareInformation.v1;

public sealed class UpdateSoftwareInformationHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSoftwareInformationCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateSoftwareInformationCommand command, CancellationToken cancellationToken)
    {
        var accessError = DeviceAccessGuard.Validate(httpContextAccessor.HttpContext?.User, command.DeviceId);
        if (accessError is not null)
        {
            return Result.Failure(accessError);
        }

        var device = await dbContext.Devices
            .Include(d => d.Software)
            .FirstOrDefaultAsync(x => x.Id == command.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure(Error.NotFound("Device not found"));
        }

        foreach (var updateSoftware in command.Software)
        {
            var software = device.Software.FirstOrDefault(s => s.Name == updateSoftware.Name);
            if (software is null)
            {
                device.Software.Add(new DeviceSoftware { Name = updateSoftware.Name });
            }
            else
            {
                software.Name = updateSoftware.Name;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
