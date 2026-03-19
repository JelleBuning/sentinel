using Sentinel.Common.DTO.Device;

namespace Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

public interface IStorageInformationRetriever
{
    public StorageInformationDto Retrieve();
}