using Mycelium.WorkerService.Common.Module.Interfaces;

namespace Mycelium.WorkerService.Core.TimeSync;

public class PlaceholderModule : IStartupModule
{
    public Task Execute(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}