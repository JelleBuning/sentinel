using Mediator;
using Mycelium.Api.Application.DTO.Device;
using Mycelium.Api.Application.Interfaces;

namespace Mycelium.Api.Application.Queries.Devices.Devices;

public class DevicesQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<DevicesQuery, GetDevicesResponse>
{
    public ValueTask<GetDevicesResponse> Handle(DevicesQuery request, CancellationToken cancellationToken)
    {
        var devices = deviceRepository.GetDevices(request.UserId);
        return ValueTask.FromResult(devices);
    }
}
