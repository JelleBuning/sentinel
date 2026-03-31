using Mediator;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Devices.RequestRemoteAccess;

public class RequestRemoteAccessCommandHandler(IDeviceMessenger deviceMessenger)
    : IRequestHandler<RequestRemoteAccessCommand>
{
    public async ValueTask<Unit> Handle(RequestRemoteAccessCommand request, CancellationToken cancellationToken)
    {
        await deviceMessenger.SendRemoteAccessRequestAsync(request.DeviceId, cancellationToken);
        return Unit.Value;
    }
}
