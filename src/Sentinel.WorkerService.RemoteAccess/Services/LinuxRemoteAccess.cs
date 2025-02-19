using Sentinel.WorkerService.RemoteAccess.Models;
using Sentinel.WorkerService.RemoteAccess.Services.Interfaces;

namespace Sentinel.WorkerService.RemoteAccess.Services;

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