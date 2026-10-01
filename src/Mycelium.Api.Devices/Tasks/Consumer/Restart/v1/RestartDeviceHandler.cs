using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.SignalR;

namespace Mycelium.Api.Devices.Tasks.Consumer.Restart.v1;

public sealed class RestartDeviceHandler(IDeviceMessenger deviceMessenger)
    : ICommandHandler<RestartDeviceCommand, Result>
{
    public async ValueTask<Result> Handle(RestartDeviceCommand command, CancellationToken cancellationToken)
    {
        await deviceMessenger.SendRestartRequestAsync(command.DeviceId, cancellationToken);
        return Result.Success();
    }
}
