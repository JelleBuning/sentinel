using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Common.Module;

public class ScheduledModuleConfig<T> : IScheduledModuleConfig<T>
{
    public TimeSpan Interval { get; set; }
}