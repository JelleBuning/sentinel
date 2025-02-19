
using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Extensions;

public static class HostExtensions
{
    public static IHost ExecuteStartupModules(this IHost host)
    {
        var startupTasks = host.Services.GetServices<IStartupModule>().ToList();
        startupTasks.ForEach(x => x.Execute(CancellationToken.None));
        return host;
    }
}