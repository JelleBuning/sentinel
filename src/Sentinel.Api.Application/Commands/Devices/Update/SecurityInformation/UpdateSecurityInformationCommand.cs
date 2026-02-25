using Mediator;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Commands.Devices.Update.SecurityInformation;

public record UpdateSecurityInformationCommand(int DeviceId, SecurityInformationDto SecurityInfo) : IRequest;
