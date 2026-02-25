using Mediator;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Queries.Devices.Devices;

public class DevicesQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<DevicesQuery, GetDevicesResponse>
{
    public ValueTask<GetDevicesResponse> Handle(DevicesQuery request, CancellationToken cancellationToken)
    {
        var devices = deviceRepository.GetDevices(request.UserId);
        return ValueTask.FromResult(devices);
    }
}
