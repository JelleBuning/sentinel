using Mediator;

namespace Sentinel.Api.Application.Commands.Devices.Ping;

public record PingDeviceCommand(int DeviceId) : IRequest;
