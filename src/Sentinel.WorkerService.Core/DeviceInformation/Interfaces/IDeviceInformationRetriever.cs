using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

public interface IDeviceInformationRetriever
{
    public GetDeviceInformationDto Retrieve();
}