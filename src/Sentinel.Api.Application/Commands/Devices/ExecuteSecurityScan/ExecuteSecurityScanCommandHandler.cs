using Mediator;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Devices.ExecuteSecurityScan;

public class ExecuteSecurityScanCommandHandler(IDeviceMessenger deviceMessenger)
    : IRequestHandler<ExecuteSecurityScanCommand>
{
    public async ValueTask<Unit> Handle(ExecuteSecurityScanCommand request, CancellationToken cancellationToken)
    {
        await deviceMessenger.SendSecurityScanRequestAsync(request.DeviceId, cancellationToken);
        return Unit.Value;
    }
}
