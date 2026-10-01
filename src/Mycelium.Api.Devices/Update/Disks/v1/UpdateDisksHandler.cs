using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Devices.Update.Disks.v1;

public sealed class UpdateDisksHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateDisksCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateDisksCommand command, CancellationToken cancellationToken)
    {
        var accessError = DeviceAccessGuard.Validate(httpContextAccessor.HttpContext?.User, command.DeviceId);
        if (accessError is not null)
        {
            return Result.Failure(accessError);
        }

        var device = await dbContext.Devices
            .Include(d => d.Disks)
            .FirstOrDefaultAsync(x => x.Id == command.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure(Error.NotFound("Device not found"));
        }

        var reportedDiskNames = command.Disks.Select(d => d.Name).ToHashSet();
        var staleDisks = device.Disks.Where(d => !reportedDiskNames.Contains(d.Name)).ToList();
        foreach (var staleDisk in staleDisks)
        {
            device.Disks.Remove(staleDisk);
            dbContext.DeviceDisks.Remove(staleDisk);
        }

        foreach (var updateDisk in command.Disks)
        {
            var disk = device.Disks.FirstOrDefault(d => d.Name == updateDisk.Name);
            if (disk is null)
            {
                device.Disks.Add(new DeviceDisk
                {
                    Name = updateDisk.Name,
                    Size = updateDisk.Size,
                    IsOsDisk = updateDisk.IsOsDisk,
                    Used = updateDisk.Used,
                    HealthStatus = updateDisk.HealthStatus
                });
            }
            else
            {
                disk.Name = updateDisk.Name;
                disk.Size = updateDisk.Size;
                disk.IsOsDisk = updateDisk.IsOsDisk;
                disk.Used = updateDisk.Used;
                disk.HealthStatus = updateDisk.HealthStatus;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
