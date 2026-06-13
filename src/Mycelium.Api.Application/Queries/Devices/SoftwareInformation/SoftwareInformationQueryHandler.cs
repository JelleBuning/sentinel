using Mediator;
using Mycelium.Api.Application.Interfaces;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Queries.Devices.SoftwareInformation;

public class SoftwareInformationQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<SoftwareInformationQuery, SoftwareInformationDto>
{
    public ValueTask<SoftwareInformationDto> Handle(SoftwareInformationQuery request, CancellationToken cancellationToken)
    {
        var softwareInfo = deviceRepository.GetSoftwareInfo(request.DeviceId);
        return ValueTask.FromResult(softwareInfo);
    }
}
