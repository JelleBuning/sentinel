using Mediator;
using Mycelium.Api.Application.Interfaces;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Queries.Devices.SecurityInformation;

public class SecurityInformationQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<SecurityInformationQuery, SecurityInformationDto>
{
    public ValueTask<SecurityInformationDto> Handle(SecurityInformationQuery request, CancellationToken cancellationToken)
    {
        var securityInfo = deviceRepository.GetSecurityInfo(request.DeviceId);
        return ValueTask.FromResult(securityInfo);
    }
}
