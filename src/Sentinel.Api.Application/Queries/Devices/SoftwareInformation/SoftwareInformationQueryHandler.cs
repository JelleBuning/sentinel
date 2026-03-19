using Mediator;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Queries.Devices.SoftwareInformation;

public class SoftwareInformationQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<SoftwareInformationQuery, SoftwareInformationDto>
{
    public ValueTask<SoftwareInformationDto> Handle(SoftwareInformationQuery request, CancellationToken cancellationToken)
    {
        var softwareInfo = deviceRepository.GetSoftwareInfo(request.DeviceId);
        return ValueTask.FromResult(softwareInfo);
    }
}
