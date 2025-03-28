using System.Management;
using Sentinel.Common.DTO.Device.Information;
using Sentinel.WorkerService.Common.Helpers;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class DeviceInformationRetriever : IDeviceInformationRetriever
{
    public GetDeviceInformationDto Retrieve()
    {
        Kernel32Helper.GetPhysicallyInstalledSystemMemory(out var memKb);
        return new GetDeviceInformationDto
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
    
    private static string GetSystemManagementString(string key, string resultKey, string scope = "")
    {
        using var searcher = new ManagementObjectSearcher(scope, "SELECT * FROM " + key);
        var res = new List<object?>();
        foreach (var mo in searcher.Get())
        {
            try
            {
                res.Add(mo.GetPropertyValue(resultKey));
            }
            catch
            {
                // ignored
            }
        }
        return string.Join(", ", res);
    }
}