using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.Windows.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.Wmi;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class FirewallSettingsRetriever(IWmiQueryService wmiQueryService) : IFirewallSettingsRetriever
{
    public FirewallSettingsDto Retrieve()
    {
        const string firewallProfileScope = @"\\.\root\StandardCimv2";
        const string firewallProfileKey = "MSFT_NetFirewallProfile";

        var rows = wmiQueryService.Query("SELECT * FROM " + firewallProfileKey, firewallProfileScope);
        var netFirewallProfiles = rows.Select(row => (
            Name: row.TryGetValue("Name", out var name) ? name?.ToString() : null,
            Enabled: row.TryGetValue("Enabled", out var enabled) ? enabled?.ToString() : null));

        return MapToFirewallSettings(netFirewallProfiles);
    }

    internal static FirewallSettingsDto MapToFirewallSettings(IEnumerable<(string? Name, string? Enabled)> profiles)
    {
        var profileList = profiles.ToList();

        return new FirewallSettingsDto
        {
            DomainFirewallEnabled = profileList.Single(x => x.Name == "Domain").Enabled == "1",
            PrivateFirewallEnabled = profileList.Single(x => x.Name == "Private").Enabled == "1",
            PublicFirewallEnabled = profileList.Single(x => x.Name == "Public").Enabled == "1",
        };
    }
}
