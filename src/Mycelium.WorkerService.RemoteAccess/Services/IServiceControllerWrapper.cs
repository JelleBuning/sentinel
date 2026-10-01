using System.ServiceProcess;

namespace Mycelium.WorkerService.RemoteAccess.Services;

public interface IServiceControllerWrapper
{
    ServiceControllerStatus Status { get; }
    void Start();
    void Stop();
    void WaitForStatus(ServiceControllerStatus status, TimeSpan timeout);
}
