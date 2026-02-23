using Mediator;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Queries.Devices.StorageInformation;

public record StorageInformationQuery(int DeviceId) : IRequest<StorageInformationDto>;
