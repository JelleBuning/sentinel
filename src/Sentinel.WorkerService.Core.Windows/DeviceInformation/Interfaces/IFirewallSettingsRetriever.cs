using Sentinel.Common.DTO.Device;

namespace Sentinel.WorkerService.Core.Windows.DeviceInformation.Interfaces;

public interface IFirewallSettingsRetriever
{
    public FirewallSettingsDto Retrieve();
}