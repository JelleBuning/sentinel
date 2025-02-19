namespace Sentinel.WorkerService.Core.TimeSync;

public interface ITimeSynchronizer
{
    Task Synchronize();
}