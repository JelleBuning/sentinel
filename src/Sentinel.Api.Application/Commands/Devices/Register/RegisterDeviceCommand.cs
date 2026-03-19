using Mediator;
using Sentinel.Api.Application.DTO.Device;

namespace Sentinel.Api.Application.Commands.Devices.Register;

public record RegisterDeviceCommand(Guid OrganisationHash, string Name) : IRequest<DeviceTokenResponse>;
