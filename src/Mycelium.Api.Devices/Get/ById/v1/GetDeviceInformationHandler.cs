using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Devices.Get.ById.v1;

public sealed class GetDeviceInformationHandler(AppDbContext dbContext)
    : IQueryHandler<GetDeviceInformationQuery, Result<InformationDto>>
{
    public async ValueTask<Result<InformationDto>> Handle(GetDeviceInformationQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.DeviceInformation)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<InformationDto>(Error.NotFound("Device not found"));
        }

        return Result.Success(new InformationDto
        {
            DeviceName = device.Name,
            OsName = device.DeviceInformation.OsName,
            OsVersion = device.DeviceInformation.OsVersion,
            Version = device.DeviceInformation.Version,
            ProductName = device.DeviceInformation.ProductName,
            Processor = device.DeviceInformation.Processor,
            InstalledRam = device.DeviceInformation.InstalledRam,
            GraphicsCard = device.DeviceInformation.GraphicsCard,
            Manufacturer = device.DeviceInformation.Manufacturer,
        });
    }
}
