using Mycelium.WorkerService.RemoteAccess.Models;
using Mycelium.WorkerService.RemoteAccess.Services.Interfaces;

namespace Mycelium.WorkerService.RemoteAccess.Services;

public class LinuxRemoteAccess : IRemoteAccessService
{
    public ConnectionDetails Start()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public bool IsRunning { get; set; }
}