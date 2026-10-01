using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Devices.Update.DeviceInformation.v1;

public sealed record UpdateDeviceInformationCommand(int DeviceId, InformationDto DeviceInfo) : ICommand<Result>;
