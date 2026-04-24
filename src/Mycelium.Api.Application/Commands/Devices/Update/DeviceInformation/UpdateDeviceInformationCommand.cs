using Mediator;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Application.Commands.Devices.Update.DeviceInformation;

public record UpdateDeviceInformationCommand(int DeviceId, UpdateDeviceInformationDto DeviceInfo) : IRequest;
