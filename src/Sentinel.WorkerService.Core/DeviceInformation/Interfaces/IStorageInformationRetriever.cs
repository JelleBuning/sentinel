using Sentinel.Common.DTO.DeviceInformation;

namespace Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

public interface IStorageInformationRetriever
{
    public StorageInformation Retrieve();
}