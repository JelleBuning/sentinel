using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Api;
using Sentinel.WorkerService.Common.Module;
using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Core.Ping;

public class PingModule(ILogger<PingModule> logger, IScheduledModuleConfig<PingModule> config, IConfiguration configuration, SentinelApiService sentinelApiService)
    : ScheduledModuleBase<PingModule>(logger, config, true)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        await sentinelApiService.PingAsync();
    }
}