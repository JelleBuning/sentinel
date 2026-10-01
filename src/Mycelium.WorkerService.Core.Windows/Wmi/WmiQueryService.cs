using System.Management;

namespace Mycelium.WorkerService.Core.Windows.Wmi;

#pragma warning disable CA1416
public sealed class WmiQueryService : IWmiQueryService
{
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Query(string query, string? scope = null)
    {
        using var searcher = string.IsNullOrEmpty(scope)
            ? new ManagementObjectSearcher(query)
            : new ManagementObjectSearcher(scope, query);

        return searcher.Get().Cast<ManagementBaseObject>()
            .Select(mo => (IReadOnlyDictionary<string, object?>)mo.Properties
                .Cast<PropertyData>()
                .ToDictionary(p => p.Name, p => p.Value, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
}
