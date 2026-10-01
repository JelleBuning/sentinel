using Microsoft.Win32;

namespace Mycelium.WorkerService.Core.Windows.Registry;

public sealed record RegistryUninstallEntry(string? DisplayName, bool IsSystemComponent, bool IsUpdate);

public interface IRegistryService
{
    IReadOnlyList<RegistryUninstallEntry> GetUninstallEntries(RegistryHive hive, string subKeyPath);
}
