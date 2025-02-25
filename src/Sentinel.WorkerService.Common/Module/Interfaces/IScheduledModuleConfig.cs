namespace Sentinel.WorkerService.Common.Module.Interfaces
{
    // ReSharper disable once UnusedTypeParameter
    public interface IScheduledModuleConfig<T>
    {
        public TimeSpan Interval { get; set; }
    }
}