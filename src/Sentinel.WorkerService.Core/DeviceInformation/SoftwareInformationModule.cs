using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Api;
using Sentinel.WorkerService.Common.Module;
using Sentinel.WorkerService.Common.Module.Interfaces;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.DeviceInformation;

public class SoftwareInformationModule(
    ILogger<SoftwareInformationModule> logger,
    IScheduledModuleConfig<SoftwareInformationModule> config,
    ISoftwareInformationRetriever softwareInformationRetriever,
    SentinelApiService sentinelApiService)
    : ScheduledModuleBase<SoftwareInformationModule>(logger, config)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var softwareInfo = softwareInformationRetriever.Retrieve();
        await sentinelApiService.UpdateSoftwareInformationAsync(softwareInfo);
    }
}