using Mediator;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Queries.Devices.SecurityInformation;

public record SecurityInformationQuery(int DeviceId) : IRequest<SecurityInformationDto>;
