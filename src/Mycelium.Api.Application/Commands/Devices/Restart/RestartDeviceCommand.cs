using Mediator;

namespace Mycelium.Api.Application.Commands.Devices.Restart;

public record RestartDeviceCommand(int DeviceId) : IRequest;
