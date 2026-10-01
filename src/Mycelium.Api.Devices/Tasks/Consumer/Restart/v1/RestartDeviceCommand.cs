using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Tasks.Consumer.Restart.v1;

public sealed record RestartDeviceCommand(int DeviceId) : ICommand<Result>;
