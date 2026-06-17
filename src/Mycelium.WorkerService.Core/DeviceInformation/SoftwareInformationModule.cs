using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.Module;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.DeviceInformation;

public class SoftwareInformationModule(
    ILogger<SoftwareInformationModule> logger,
    IScheduledModuleConfig<SoftwareInformationModule> config,
    ISoftwareInformationRetriever softwareInformationRetriever,
    MyceliumApiService MyceliumApiService)
    : ScheduledModuleBase<SoftwareInformationModule>(logger, config)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var softwareInfo = softwareInformationRetriever.Retrieve();
        await MyceliumApiService.UpdateSoftwareInformationAsync(softwareInfo);
    }
}