using Mycelium.WorkerService.Core.TimeSync;

namespace Mycelium.WorkerService.Core.Linux.TimeSync;

public class LinuxTimeSync : ITimeSynchronizer
{
    public Task Synchronize()
    {
        throw new NotImplementedException();
    }
}