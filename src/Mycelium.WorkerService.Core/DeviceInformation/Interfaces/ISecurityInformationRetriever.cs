using Mycelium.Common.DTO.Device;

namespace Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

public interface ISecurityInformationRetriever
{
    public SecurityInformationDto Retrieve();
}