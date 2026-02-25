using Mediator;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Devices.Ping;

public class PingDeviceCommandHandler(IDeviceRepository deviceRepository) : IRequestHandler<PingDeviceCommand>
{
    public ValueTask<Unit> Handle(PingDeviceCommand request, CancellationToken cancellationToken)
    {
        deviceRepository.Ping(request.DeviceId);
        return ValueTask.FromResult(Unit.Value);
    }
}
