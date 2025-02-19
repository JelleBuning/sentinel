using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Api;
using Sentinel.WorkerService.Common.Module;
using Sentinel.WorkerService.Common.Module.Interfaces;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.DeviceInformation;

public class StorageInformationModule(
    ILogger<StorageInformationModule> logger,
    IScheduledModuleConfig<StorageInformationModule> config,
    IStorageInformationRetriever storageInformationRetriever,
    SentinelApiService sentinelApiService)
    : ScheduledModuleBase<StorageInformationModule>(logger, config)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var storageInfo = storageInformationRetriever.Retrieve();
        await sentinelApiService.UpdateStorageInformationAsync(storageInfo);
    }
}