namespace Mycelium.WorkerService.Core.TimeSync;

public interface ITimeSynchronizer
{
    Task Synchronize();
}