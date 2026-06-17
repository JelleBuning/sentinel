using Mediator;
using Mycelium.Api.Application.DTO.Device;

namespace Mycelium.Api.Application.Commands.Devices.Register;

public record RegisterDeviceCommand(Guid OrganisationHash, string Name) : IRequest<DeviceTokenResponse>;
