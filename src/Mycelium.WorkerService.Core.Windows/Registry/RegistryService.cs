using Microsoft.Win32;

namespace Mycelium.WorkerService.Core.Windows.Registry;

#pragma warning disable CA1416
public sealed class RegistryService : IRegistryService
{
    public IReadOnlyList<RegistryUninstallEntry> GetUninstallEntries(RegistryHive hive, string subKeyPath)
    {
        using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
        using var uninstallKey = baseKey.OpenSubKey(subKeyPath);
        if (uninstallKey is null)
        {
            return [];
        }

        var entries = new List<RegistryUninstallEntry>();
        foreach (var subKeyName in uninstallKey.GetSubKeyNames())
        {
            using var subKey = uninstallKey.OpenSubKey(subKeyName);
            var displayName = subKey?.GetValue("DisplayName") as string;
            var isSystemComponent = subKey?.GetValue("SystemComponent") is int systemComponent && systemComponent == 1;
            var isUpdate = subKey?.GetValue("ParentKeyName") is not null;

            entries.Add(new RegistryUninstallEntry(displayName, isSystemComponent, isUpdate));
        }

        return entries;
    }
}
