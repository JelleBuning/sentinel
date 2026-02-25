using Mediator;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Queries.Devices.SecurityInformation;

public class SecurityInformationQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<SecurityInformationQuery, SecurityInformationDto>
{
    public ValueTask<SecurityInformationDto> Handle(SecurityInformationQuery request, CancellationToken cancellationToken)
    {
        var securityInfo = deviceRepository.GetSecurityInfo(request.DeviceId);
        return ValueTask.FromResult(securityInfo);
    }
}
