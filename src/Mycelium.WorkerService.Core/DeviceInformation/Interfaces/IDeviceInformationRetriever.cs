using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

public interface IDeviceInformationRetriever
{
    public GetDeviceInformationDto Retrieve();
}