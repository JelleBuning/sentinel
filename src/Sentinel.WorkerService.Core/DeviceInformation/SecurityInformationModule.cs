using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Api;
using Sentinel.WorkerService.Common.Module;
using Sentinel.WorkerService.Common.Module.Interfaces;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.DeviceInformation;

public class SecurityInformationModule(
    ILogger<SecurityInformationModule> logger,
    IScheduledModuleConfig<SecurityInformationModule> config,
    ISecurityInformationRetriever securityInformationRetriever,
    SentinelApiService sentinelApiService)
    : ScheduledModuleBase<SecurityInformationModule>(logger, config, runImmediately: true)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var securityInfo = securityInformationRetriever.Retrieve();
        await sentinelApiService.UpdateSecurityInformationAsync(securityInfo);
    }
}