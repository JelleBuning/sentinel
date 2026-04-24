using Mediator;

namespace Mycelium.Api.Application.Commands.Devices.Update.StorageInformation;

public record UpdateStorageInformationCommand(int DeviceId, Mycelium.Common.DTO.Device.StorageInformationDto StorageInfo) : IRequest;
