using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Tasks.Consumer.ExecuteSecurityScan.v1;

public sealed record ExecuteSecurityScanCommand(int DeviceId) : ICommand<Result>;
