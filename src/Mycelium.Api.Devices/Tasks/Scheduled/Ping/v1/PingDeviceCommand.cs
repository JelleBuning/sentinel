using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Tasks.Scheduled.Ping.v1;

public sealed record PingDeviceCommand(int DeviceId) : ICommand<Result>;
