using Mediator;
using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.Api.Application.Queries.Devices.DeviceInformation;

public record DeviceInformationQuery(int DeviceId) : IRequest<GetDeviceInformationDto>;
