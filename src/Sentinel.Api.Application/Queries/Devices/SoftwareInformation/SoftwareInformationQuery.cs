using Mediator;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Queries.Devices.SoftwareInformation;

public record SoftwareInformationQuery(int DeviceId) : IRequest<SoftwareInformationDto>;
