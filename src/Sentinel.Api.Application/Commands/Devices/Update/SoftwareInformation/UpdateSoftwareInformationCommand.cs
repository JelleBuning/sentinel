using Mediator;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Application.Commands.Devices.Update.SoftwareInformation;

public record UpdateSoftwareInformationCommand(int DeviceId, SoftwareInformationDto SoftwareInfo) : IRequest;
