using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Devices.Tasks.Scheduled.Ping.v1;

public sealed class PingDeviceHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<PingDeviceCommand, Result>
{
    public async ValueTask<Result> Handle(PingDeviceCommand command, CancellationToken cancellationToken)
    {
        var accessError = DeviceAccessGuard.Validate(httpContextAccessor.HttpContext?.User, command.DeviceId);
        if (accessError is not null)
        {
            return Result.Failure(accessError);
        }

        var device = await dbContext.Devices.FirstOrDefaultAsync(x => x.Id == command.DeviceId, cancellationToken);
        if (device is null)
        {
            return Result.Failure(Error.NotFound("Device not found"));
        }

        device.LastActive = DateTime.Now;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
