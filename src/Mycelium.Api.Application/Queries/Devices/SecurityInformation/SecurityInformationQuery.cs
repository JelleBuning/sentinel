using Mediator;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Queries.Devices.SecurityInformation;

public record SecurityInformationQuery(int DeviceId) : IRequest<SecurityInformationDto>;
