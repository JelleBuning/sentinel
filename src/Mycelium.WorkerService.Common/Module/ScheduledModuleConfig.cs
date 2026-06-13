using Mycelium.WorkerService.Common.Module.Interfaces;

namespace Mycelium.WorkerService.Common.Module;

public class ScheduledModuleConfig<T> : IScheduledModuleConfig<T>
{
    public TimeSpan Interval { get; set; }
}