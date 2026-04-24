using Mediator;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Queries.Devices.StorageInformation;

public record StorageInformationQuery(int DeviceId) : IRequest<StorageInformationDto>;
