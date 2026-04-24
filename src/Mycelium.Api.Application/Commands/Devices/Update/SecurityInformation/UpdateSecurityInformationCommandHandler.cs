using Mediator;
using Mycelium.Api.Application.Interfaces;

namespace Mycelium.Api.Application.Commands.Devices.Update.SecurityInformation;

public class UpdateSecurityInformationCommandHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<UpdateSecurityInformationCommand>
{
    public ValueTask<Unit> Handle(UpdateSecurityInformationCommand request, CancellationToken cancellationToken)
    {
        deviceRepository.UpdateSecurityInfo(request.DeviceId, request.SecurityInfo);
        return ValueTask.FromResult(Unit.Value);
    }
}
