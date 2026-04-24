using Mycelium.Common.DTO.Device;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation.Interfaces;

public interface IFirewallSettingsRetriever
{
    public FirewallSettingsDto Retrieve();
}