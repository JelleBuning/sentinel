using Mediator;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Devices.Update.SoftwareInformation;

public class UpdateSoftwareInformationCommandHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<UpdateSoftwareInformationCommand>
{
    public ValueTask<Unit> Handle(UpdateSoftwareInformationCommand request, CancellationToken cancellationToken)
    {
        deviceRepository.UpdateSoftwareInfo(request.DeviceId, request.SoftwareInfo);
        return ValueTask.FromResult(Unit.Value);
    }
}
