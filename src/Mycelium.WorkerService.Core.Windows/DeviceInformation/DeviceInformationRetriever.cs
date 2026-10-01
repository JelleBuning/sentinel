using Mycelium.Common.DTO.Device.Information;
using Mycelium.WorkerService.Common.Hardware;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.Wmi;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class DeviceInformationRetriever(IWmiQueryService wmiQueryService, IMemoryInfoProvider memoryInfoProvider) : IDeviceInformationRetriever
{
    public InformationDto Retrieve()
    {
        var memKb = memoryInfoProvider.GetInstalledMemoryKilobytes();
        return new InformationDto
        {
            DeviceName = Environment.MachineName,
            OsName = GetSystemManagementString("Win32_OperatingSystem", "Caption"),
            OsVersion = Environment.OSVersion.VersionString,
            Version = Environment.Version.ToString(),

            Manufacturer = GetSystemManagementString("Win32_ComputerSystem", "Manufacturer"),
            ProductName = GetSystemManagementString("Win32_ComputerSystemProduct", "Name"),
            InstalledRam = (memKb / 1024 / 1024).ToString(),
            Processor = GetSystemManagementString("Win32_Processor", "Name"),
            GraphicsCard = GetSystemManagementString("Win32_VideoController", "Caption")
        };
    }

    private string GetSystemManagementString(string key, string resultKey, string scope = "")
    {
        var rows = wmiQueryService.Query("SELECT * FROM " + key, scope);
        var res = new List<object?>();
        foreach (var row in rows)
        {
            try
            {
                res.Add(row[resultKey]);
            }
            catch
            {
                // ignored
            }
        }
        return string.Join(", ", res);
    }
}
