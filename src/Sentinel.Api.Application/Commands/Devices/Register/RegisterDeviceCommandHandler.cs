using Mediator;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Devices.Register;

public class RegisterDeviceCommandHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<RegisterDeviceCommand, DeviceTokenResponse>
{
    public ValueTask<DeviceTokenResponse> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        var response = deviceRepository.Register(request.OrganisationHash, request.Name);
        return ValueTask.FromResult(response);
    }
}
