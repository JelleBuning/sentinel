using Mediator;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Queries.Devices.StorageInformation;

public class StorageInformationQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<StorageInformationQuery, StorageInformationDto>
{
    public ValueTask<StorageInformationDto> Handle(StorageInformationQuery request, CancellationToken cancellationToken)
    {
        var storageInfo = deviceRepository.GetStorageInfo(request.DeviceId);
        return ValueTask.FromResult(storageInfo);
    }
}
