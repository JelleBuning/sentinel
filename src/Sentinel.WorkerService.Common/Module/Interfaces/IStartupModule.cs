namespace Sentinel.WorkerService.Common.Module.Interfaces;

public interface IStartupModule : IModule
{
    public Task Execute(CancellationToken cancellationToken);
}