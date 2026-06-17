using Mediator;
using Mycelium.Api.Application.Interfaces;

namespace Mycelium.Api.Application.Commands.Devices.Update.DeviceInformation;

public class UpdateDeviceInformationCommandHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<UpdateDeviceInformationCommand>
{
    public ValueTask<Unit> Handle(UpdateDeviceInformationCommand request, CancellationToken cancellationToken)
    {
        deviceRepository.UpdateDeviceInformation(request.DeviceId, request.DeviceInfo);
        return ValueTask.FromResult(Unit.Value);
    }
}
