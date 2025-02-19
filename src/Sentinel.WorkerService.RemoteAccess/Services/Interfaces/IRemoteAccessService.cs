using Sentinel.WorkerService.RemoteAccess.Models;

namespace Sentinel.WorkerService.RemoteAccess.Services.Interfaces;

public interface IRemoteAccessService
{
    public bool IsRunning { get; }
    public ConnectionDetails Start();
    public void Stop();
}