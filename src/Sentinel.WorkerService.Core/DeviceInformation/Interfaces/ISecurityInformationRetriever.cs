using Sentinel.Common.DTO.Device;

namespace Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

public interface ISecurityInformationRetriever
{
    public SecurityInformationDto Retrieve();
}