using Mediator;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.Api.Application.Queries.Devices.DeviceInformation;

public class DeviceInformationQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<DeviceInformationQuery, GetDeviceInformationDto>
{
    public ValueTask<GetDeviceInformationDto> Handle(DeviceInformationQuery request, CancellationToken cancellationToken)
    {
        var deviceInfo = deviceRepository.GetDeviceInformation(request.DeviceId);
        return ValueTask.FromResult(deviceInfo);
    }
}
