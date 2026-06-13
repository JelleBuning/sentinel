using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.Module;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.DeviceInformation;

public class SecurityInformationModule(
    ILogger<SecurityInformationModule> logger,
    IScheduledModuleConfig<SecurityInformationModule> config,
    ISecurityInformationRetriever securityInformationRetriever,
    MyceliumApiService MyceliumApiService)
    : ScheduledModuleBase<SecurityInformationModule>(logger, config, runImmediately: true)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var securityInfo = securityInformationRetriever.Retrieve();
        await MyceliumApiService.UpdateSecurityInformationAsync(securityInfo);
    }
}