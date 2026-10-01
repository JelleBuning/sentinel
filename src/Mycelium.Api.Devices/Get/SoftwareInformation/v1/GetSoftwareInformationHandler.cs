using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Get.SoftwareInformation.v1;

public sealed class GetSoftwareInformationHandler(AppDbContext dbContext)
    : IQueryHandler<GetSoftwareInformationQuery, Result<List<SoftwareDto>>>
{
    public async ValueTask<Result<List<SoftwareDto>>> Handle(GetSoftwareInformationQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.Software)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<List<SoftwareDto>>(Error.NotFound("Device not found"));
        }

        return Result.Success(device.Software
            .Select(software => new SoftwareDto { Name = software.Name })
            .OrderBy(software => software.Name)
            .ToList());
    }
}
