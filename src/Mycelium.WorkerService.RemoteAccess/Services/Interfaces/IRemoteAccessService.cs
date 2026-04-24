using Mycelium.WorkerService.RemoteAccess.Models;

namespace Mycelium.WorkerService.RemoteAccess.Services.Interfaces;

public interface IRemoteAccessService
{
    public bool IsRunning { get; }
    public ConnectionDetails Start();
    public void Stop();
}