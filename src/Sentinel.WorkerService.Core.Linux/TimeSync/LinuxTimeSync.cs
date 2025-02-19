using Sentinel.WorkerService.Core.TimeSync;

namespace Sentinel.WorkerService.Core.Linux.TimeSync;

public class LinuxTimeSync : ITimeSynchronizer
{
    public Task Synchronize()
    {
        throw new NotImplementedException();
    }
}