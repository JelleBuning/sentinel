using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.Module;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.DeviceInformation;

public class DiskInformationModule(
    ILogger<DiskInformationModule> logger,
    IScheduledModuleConfig<DiskInformationModule> config,
    IDiskInformationRetriever diskInformationRetriever,
    MyceliumApiService myceliumApiService)
    : ScheduledModuleBase<DiskInformationModule>(logger, config)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var disks = diskInformationRetriever.Retrieve();
        await myceliumApiService.UpdateDiskInformationAsync(disks);
    }
}