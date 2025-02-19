using Sentinel.Common.DTO.DeviceInformation;

namespace Sentinel.WorkerService.Core.Windows.DeviceInformation.Interfaces;

public interface IFirewallSettingsRetriever
{
    public FirewallSettings Retrieve();
}