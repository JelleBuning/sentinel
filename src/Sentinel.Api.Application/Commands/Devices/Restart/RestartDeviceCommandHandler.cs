using Mediator;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Devices.Restart;

public class RestartDeviceCommandHandler(IDeviceMessenger deviceMessenger)
    : IRequestHandler<RestartDeviceCommand>
{
    public async ValueTask<Unit> Handle(RestartDeviceCommand request, CancellationToken cancellationToken)
    {
        await deviceMessenger.SendRestartRequestAsync(request.DeviceId, cancellationToken);
        return Unit.Value;
    }
}
