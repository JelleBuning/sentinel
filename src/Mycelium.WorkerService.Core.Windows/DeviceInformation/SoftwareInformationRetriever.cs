using System.Management;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class SoftwareInformationRetriever : ISoftwareInformationRetriever
{
    public SoftwareInformationDto Retrieve()
    {
        var profileKey = "Win32_Product";
        var mos = new ManagementObjectSearcher("SELECT * FROM " + profileKey);
        
        return new SoftwareInformationDto
        {
            Software = mos.Get().Cast<ManagementObject>()
                .Where(x => x?["Name"]?.ToString() != null)
                .Select(x => new SoftwareDto
                {
                    Name = x["Name"].ToString()!
                }).ToList()
        };
    }
}