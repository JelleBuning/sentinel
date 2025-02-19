using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Core.TimeSync;

public class TimeSyncModule(ITimeSynchronizer timeSynchronizer, ILogger<TimeSyncModule> logger) : IStartupModule
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        logger.LogInformation("[>] Syncing time");
        await timeSynchronizer.Synchronize();
    }
}