using Microsoft.Win32;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.Registry;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class SoftwareInformationRetriever(IRegistryService registryService) : ISoftwareInformationRetriever
{
    private static readonly (RegistryHive Hive, string SubKey)[] UninstallKeys =
    [
        (RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
        (RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"),
        (RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
    ];

    public List<SoftwareDto> Retrieve()
    {
        return UninstallKeys
            .SelectMany(x => ReadNames(x.Hive, x.SubKey))
            .Distinct()
            .Select(name => new SoftwareDto { Name = name })
            .ToList();
    }

    private IEnumerable<string> ReadNames(RegistryHive hive, string subKeyPath)
    {
        return registryService.GetUninstallEntries(hive, subKeyPath)
            .Where(entry => ShouldInclude(entry.DisplayName, entry.IsSystemComponent, entry.IsUpdate))
            .Select(entry => entry.DisplayName!);
    }

    internal static bool ShouldInclude(string? name, bool isSystemComponent, bool isUpdate)
    {
        return !string.IsNullOrWhiteSpace(name) && !isSystemComponent && !isUpdate;
    }
}
