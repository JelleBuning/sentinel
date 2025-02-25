﻿using System.Globalization;
using System.Management;
using Sentinel.Common.DTO.DeviceInformation;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;
using Sentinel.WorkerService.Core.Windows.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class SecurityInformationRetriever(IFirewallSettingsRetriever firewallSettingsRetriever) : ISecurityInformationRetriever
{
    public SecurityInformation Retrieve()
    {
        const string defenderScope = @"\\.\root\Microsoft\Windows\Defender";
        const string computerStatusKey = "MSFT_MpComputerStatus";
        using var managementObjectSearcher = new ManagementObjectSearcher(defenderScope, "SELECT * FROM " + computerStatusKey);
        var managementBaseObject = managementObjectSearcher.Get().Cast<ManagementBaseObject>().Single();
        var securityInformation = new SecurityInformation
        {
            AntivirusEnabled = (bool)managementBaseObject["AntiVirusEnabled"],
            LastAntivirusUpdate = ParseExact(managementBaseObject["AntivirusSignatureLastUpdated"]),
            LastAntispywareUpdate = ParseExact(managementBaseObject["AntiSpywareSignatureLastUpdated"]),
            RealTimeProtectionEnabled = (bool)managementBaseObject["RealTimeProtectionEnabled"],
            NisEnabled = (bool)managementBaseObject["NISEnabled"],
            TamperProtectionEnabled = (bool)managementBaseObject["IsTamperProtected"],
            AntispywareEnabled = (bool)managementBaseObject["AntiSpywareEnabled"],
            IsVirtualMachine = (bool)managementBaseObject["IsVirtualMachine"],
            LastSecurityScan = new LastSecurityScan
            {
                // TODO: fix, cant find the properties
                // LastScan = ParseExact(managementBaseObject["QuickScanStartTime"]),
                // Duration = ParseExact(managementBaseObject["QuickScanEndTime"]) - ParseExact(managementBaseObject["QuickScanStartTime"]) 
            },
            FirewallSettings = firewallSettingsRetriever.Retrieve()
        };
        
        return securityInformation;
    }

    private static DateTime ParseExact(object mo)
    {
        return DateTime.ParseExact(mo.ToString() ?? string.Empty, "yyyyMMddHHmmss.ffffff'+000'", CultureInfo.InvariantCulture);
    }
}