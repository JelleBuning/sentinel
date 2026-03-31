using Mediator;

namespace Sentinel.Api.Application.Commands.Devices.Update.StorageInformation;

public record UpdateStorageInformationCommand(int DeviceId, Sentinel.Common.DTO.Device.StorageInformationDto StorageInfo) : IRequest;
