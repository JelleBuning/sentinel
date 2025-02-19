using Sentinel.Common.DTO.DeviceInformation;

namespace Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

public interface ISoftwareInformationRetriever
{
    public SoftwareInformation Retrieve();
}