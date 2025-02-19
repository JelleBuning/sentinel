namespace Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

public interface IDeviceInformationRetriever
{
    public Sentinel.Common.DTO.DeviceInformation.DeviceInformation Retrieve();
}