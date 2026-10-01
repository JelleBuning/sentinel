using Mycelium.Common.DTO.Device;

namespace Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

public interface IDiskInformationRetriever
{
    public List<DiskDto> Retrieve();
}
