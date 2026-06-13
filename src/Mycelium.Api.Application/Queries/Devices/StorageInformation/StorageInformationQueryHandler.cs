using Mediator;
using Mycelium.Api.Application.Interfaces;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Queries.Devices.StorageInformation;

public class StorageInformationQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<StorageInformationQuery, StorageInformationDto>
{
    public ValueTask<StorageInformationDto> Handle(StorageInformationQuery request, CancellationToken cancellationToken)
    {
        var storageInfo = deviceRepository.GetStorageInfo(request.DeviceId);
        return ValueTask.FromResult(storageInfo);
    }
}
