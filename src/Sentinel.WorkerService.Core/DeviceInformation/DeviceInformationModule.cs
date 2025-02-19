using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Api;
using Sentinel.WorkerService.Common.Module;
using Sentinel.WorkerService.Common.Module.Interfaces;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.DeviceInformation;

public class DeviceInformationModule(
    ILogger<DeviceInformationModule> logger,
    IScheduledModuleConfig<DeviceInformationModule> config,
    IDeviceInformationRetriever deviceInformationRetriever,
    SentinelApiService sentinelApiService)
    : ScheduledModuleBase<DeviceInformationModule>(logger, config, runImmediately: true)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var deviceInfo = deviceInformationRetriever.Retrieve();
        await sentinelApiService.UpdateDeviceInformationAsync(deviceInfo);
    }
}