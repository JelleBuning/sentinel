using Mediator;
using Mycelium.Api.Application.Interfaces;

namespace Mycelium.Api.Application.Commands.Devices.Ping;

public class PingDeviceCommandHandler(IDeviceRepository deviceRepository) : IRequestHandler<PingDeviceCommand>
{
    public ValueTask<Unit> Handle(PingDeviceCommand request, CancellationToken cancellationToken)
    {
        deviceRepository.Ping(request.DeviceId);
        return ValueTask.FromResult(Unit.Value);
    }
}
