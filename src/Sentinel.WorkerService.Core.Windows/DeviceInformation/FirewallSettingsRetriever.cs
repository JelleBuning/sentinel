using System.Management;
using Sentinel.Common.DTO.DeviceInformation;
using Sentinel.WorkerService.Core.Windows.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class FirewallSettingsRetriever : IFirewallSettingsRetriever
{
    public FirewallSettings Retrieve()
    {
        const string firewallProfileScope = @"\\.\root\StandardCimv2";
        const string firewallProfileKey = "MSFT_NetFirewallProfile";
        using var firewallObjectSearcher = new ManagementObjectSearcher(firewallProfileScope, "SELECT * FROM " + firewallProfileKey);
        var netFirewallProfiles = firewallObjectSearcher.Get().Cast<ManagementBaseObject>().ToList();

        var firewallSettings = new FirewallSettings()
        {
            DomainFirewallEnabled = netFirewallProfiles.Single(x => x["Name"]?.ToString() == "Domain")["Enabled"]?.ToString() == "1",
            PrivateFirewallEnabled = netFirewallProfiles.Single(x => x["Name"]?.ToString() == "Private")["Enabled"]?.ToString() == "1",
            PublicFirewallEnabled = netFirewallProfiles.Single(x => x["Name"]?.ToString() == "Public")["Enabled"]?.ToString() == "1",
        };
        return firewallSettings;
    }
}