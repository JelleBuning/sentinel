using Mycelium.Common.DTO.Device;

namespace Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

public interface IStorageInformationRetriever
{
    public StorageInformationDto Retrieve();
}