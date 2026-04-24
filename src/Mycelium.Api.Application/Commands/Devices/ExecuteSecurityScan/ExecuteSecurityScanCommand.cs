using Mediator;

namespace Mycelium.Api.Application.Commands.Devices.ExecuteSecurityScan;

public record ExecuteSecurityScanCommand(int DeviceId) : IRequest;
