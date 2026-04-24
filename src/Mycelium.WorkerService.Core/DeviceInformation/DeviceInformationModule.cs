using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.Module;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.DeviceInformation;

public class DeviceInformationModule(
    ILogger<DeviceInformationModule> logger,
    IScheduledModuleConfig<DeviceInformationModule> config,
    IDeviceInformationRetriever deviceInformationRetriever,
    MyceliumApiService MyceliumApiService)
    : ScheduledModuleBase<DeviceInformationModule>(logger, config, runImmediately: true)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var deviceInfo = deviceInformationRetriever.Retrieve();
        await MyceliumApiService.UpdateDeviceInformationAsync(deviceInfo);
    }
}