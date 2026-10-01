using System.Globalization;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.Wmi;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class SecurityInformationRetriever(IWmiQueryService wmiQueryService, IFirewallSettingsRetriever firewallSettingsRetriever) : ISecurityInformationRetriever
{
    public SecurityDto Retrieve()
    {
        const string defenderScope = @"\\.\root\Microsoft\Windows\Defender";
        const string computerStatusKey = "MSFT_MpComputerStatus";

        var managementBaseObject = wmiQueryService.Query("SELECT * FROM " + computerStatusKey, defenderScope).Single();

        var securityInformation = new SecurityDto
        {
            AntivirusEnabled = managementBaseObject["AntivirusEnabled"] is true,
            LastAntivirusUpdate = ParseExact(managementBaseObject["AntivirusSignatureLastUpdated"]),
            LastAntispywareUpdate = ParseExact(managementBaseObject["AntispywareSignatureLastUpdated"]),
            RealTimeProtectionEnabled = managementBaseObject["RealTimeProtectionEnabled"] is true,
            NisEnabled = managementBaseObject["NISEnabled"] is true,
            TamperProtectionEnabled = managementBaseObject["IsTamperProtected"] is true,
            AntispywareEnabled = managementBaseObject["AntispywareEnabled"] is true,
            IsVirtualMachine = managementBaseObject["IsVirtualMachine"] is true,
            LastSecurityScanDto = new LastSecurityScanDto
            {
                // TODO: fix, cant find the properties
                // LastScan = ParseExact(managementBaseObject["QuickScanStartTime"]),
                // Duration = ParseExact(managementBaseObject["QuickScanEndTime"]) - ParseExact(managementBaseObject["QuickScanStartTime"])
            },
            FirewallSettingsDto = firewallSettingsRetriever.Retrieve()
        };

        return securityInformation;
    }

    internal static DateTime ParseExact(object? mo)
    {
        return DateTime.ParseExact(mo?.ToString() ?? string.Empty, "yyyyMMddHHmmss.ffffff'+000'", CultureInfo.InvariantCulture);
    }
}
