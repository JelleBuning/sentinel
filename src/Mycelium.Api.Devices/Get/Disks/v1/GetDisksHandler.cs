using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Get.Disks.v1;

public sealed class GetDisksHandler(AppDbContext dbContext)
    : IQueryHandler<GetDisksQuery, Result<List<DiskDto>>>
{
    public async ValueTask<Result<List<DiskDto>>> Handle(GetDisksQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.Disks)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<List<DiskDto>>(Error.NotFound("Device not found"));
        }

        return Result.Success(device.Disks.Select(disk => new DiskDto
        {
            Name = disk.Name,
            IsOsDisk = disk.IsOsDisk,
            Used = disk.Used,
            Size = disk.Size,
            HealthStatus = disk.HealthStatus
        }).ToList());
    }
}
