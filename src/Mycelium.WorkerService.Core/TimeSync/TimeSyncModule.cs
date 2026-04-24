using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Module.Interfaces;

namespace Mycelium.WorkerService.Core.TimeSync;

public class TimeSyncModule(ITimeSynchronizer timeSynchronizer, ILogger<TimeSyncModule> logger) : IStartupModule
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        logger.LogInformation("[>] Syncing time");
        await timeSynchronizer.Synchronize();
    }
}