using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.Module;
using Mycelium.WorkerService.Common.Module.Interfaces;

namespace Mycelium.WorkerService.Core.Ping;

public class PingModule(ILogger<PingModule> logger, IScheduledModuleConfig<PingModule> config, MyceliumApiService MyceliumApiService)
    : ScheduledModuleBase<PingModule>(logger, config, true)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        await MyceliumApiService.PingAsync();
    }
}