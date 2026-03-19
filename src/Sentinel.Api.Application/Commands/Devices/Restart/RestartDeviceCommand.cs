using Mediator;

namespace Sentinel.Api.Application.Commands.Devices.Restart;

public record RestartDeviceCommand(int DeviceId) : IRequest;
