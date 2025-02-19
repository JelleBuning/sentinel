namespace Sentinel.WorkerService.Common.Module.Interfaces
{
    public interface IScheduledModuleConfig<T>
    {
        public TimeSpan Interval { get; set; }
    }
}