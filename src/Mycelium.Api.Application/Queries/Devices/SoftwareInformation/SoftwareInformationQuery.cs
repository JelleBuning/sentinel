using Mediator;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Queries.Devices.SoftwareInformation;

public record SoftwareInformationQuery(int DeviceId) : IRequest<SoftwareInformationDto>;
