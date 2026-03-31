using Mediator;
using Sentinel.Api.Application.DTO.Device;

namespace Sentinel.Api.Application.Queries.Devices.Devices;

public record DevicesQuery(int UserId) : IRequest<GetDevicesResponse>;
