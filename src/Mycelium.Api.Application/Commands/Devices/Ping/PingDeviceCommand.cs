using Mediator;

namespace Mycelium.Api.Application.Commands.Devices.Ping;

public record PingDeviceCommand(int DeviceId) : IRequest;
