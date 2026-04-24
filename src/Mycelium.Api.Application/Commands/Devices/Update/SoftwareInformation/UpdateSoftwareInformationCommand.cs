using Mediator;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Application.Commands.Devices.Update.SoftwareInformation;

public record UpdateSoftwareInformationCommand(int DeviceId, SoftwareInformationDto SoftwareInfo) : IRequest;
