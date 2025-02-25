
using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Extensions;

public static class HostExtensions
{
    public static IHost ExecuteStartupModules(this IHost host)
    {
        var startupTasks = host.Services.GetServices<IStartupModule>().ToList();
        startupTasks.ForEach(startupModule =>
        {
            try
            {
                startupModule.Execute(CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error executing startup module {startupModule.GetType().Name}: {e.Message}");
            }
        });
        return host;
    }
}