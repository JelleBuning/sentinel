using Mediator;
using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.Api.Application.Commands.Devices.Update.DeviceInformation;

public record UpdateDeviceInformationCommand(int DeviceId, UpdateDeviceInformationDto DeviceInfo) : IRequest;
