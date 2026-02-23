using Mediator;

namespace Sentinel.Api.Application.Commands.Devices.RequestRemoteAccess;

public record RequestRemoteAccessCommand(int DeviceId) : IRequest;
