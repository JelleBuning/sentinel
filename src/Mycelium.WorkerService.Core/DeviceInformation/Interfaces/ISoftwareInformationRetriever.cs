using Mycelium.Common.DTO.Device;

namespace Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

public interface ISoftwareInformationRetriever
{
    public SoftwareInformationDto Retrieve();
}