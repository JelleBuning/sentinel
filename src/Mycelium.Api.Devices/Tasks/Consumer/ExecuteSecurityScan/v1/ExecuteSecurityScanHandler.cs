using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.SignalR;

namespace Mycelium.Api.Devices.Tasks.Consumer.ExecuteSecurityScan.v1;

public sealed class ExecuteSecurityScanHandler(IDeviceMessenger deviceMessenger)
    : ICommandHandler<ExecuteSecurityScanCommand, Result>
{
    public async ValueTask<Result> Handle(ExecuteSecurityScanCommand command, CancellationToken cancellationToken)
    {
        await deviceMessenger.SendSecurityScanRequestAsync(command.DeviceId, cancellationToken);
        return Result.Success();
    }
}
