using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.SignalR;

namespace Mycelium.Api.Devices.Tasks.Consumer.RequestRemoteAccess.v1;

public sealed class RequestRemoteAccessHandler(IDeviceMessenger deviceMessenger)
    : ICommandHandler<RequestRemoteAccessCommand, Result>
{
    public async ValueTask<Result> Handle(RequestRemoteAccessCommand command, CancellationToken cancellationToken)
    {
        await deviceMessenger.SendRemoteAccessRequestAsync(command.DeviceId, cancellationToken);
        return Result.Success();
    }
}
