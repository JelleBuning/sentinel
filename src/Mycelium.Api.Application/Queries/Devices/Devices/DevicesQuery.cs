using Mediator;
using Mycelium.Api.Application.DTO.Device;

namespace Mycelium.Api.Application.Queries.Devices.Devices;

public record DevicesQuery(int UserId) : IRequest<GetDevicesResponse>;
