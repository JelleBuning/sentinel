using Mediator;

namespace Sentinel.Api.Application.Commands.Devices.ExecuteSecurityScan;

public record ExecuteSecurityScanCommand(int DeviceId) : IRequest;
