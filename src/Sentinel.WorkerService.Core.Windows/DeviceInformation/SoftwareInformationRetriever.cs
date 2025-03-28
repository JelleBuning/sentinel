using System.Management;
using Sentinel.Common.DTO.Device;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class SoftwareInformationRetriever : ISoftwareInformationRetriever
{
    public SoftwareInformation Retrieve()
    {
        var profileKey = "Win32_Product";
        var mos = new ManagementObjectSearcher("SELECT * FROM " + profileKey);
        
        return new SoftwareInformation
        {
            Software = mos.Get().Cast<ManagementObject>()
                .Where(x => x?["Name"]?.ToString() != null)
                .Select(x => new Software
                {
                    Name = x["Name"].ToString()!
                }).ToList()
        };
    }
}