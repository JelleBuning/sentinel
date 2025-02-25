using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Core.TimeSync;

public class PlaceholderModule : IStartupModule
{
    public Task Execute(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}