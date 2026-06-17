using Mediator;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Application.Queries.Devices.DeviceInformation;

public record DeviceInformationQuery(int DeviceId) : IRequest<GetDeviceInformationDto>;
