using Mediator;

namespace Mycelium.Api.Application.Commands.Devices.RequestRemoteAccess;

public record RequestRemoteAccessCommand(int DeviceId) : IRequest;
