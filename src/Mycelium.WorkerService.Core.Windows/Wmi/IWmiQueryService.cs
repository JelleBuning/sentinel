namespace Mycelium.WorkerService.Core.Windows.Wmi;

public interface IWmiQueryService
{
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Query(string query, string? scope = null);
}
