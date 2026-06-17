using Mediator;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Commands.Devices.Update.SecurityInformation;

public record UpdateSecurityInformationCommand(int DeviceId, SecurityInformationDto SecurityInfo) : IRequest;
