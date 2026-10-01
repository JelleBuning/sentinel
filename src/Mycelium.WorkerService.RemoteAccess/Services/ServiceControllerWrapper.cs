using System.ServiceProcess;

namespace Mycelium.WorkerService.RemoteAccess.Services;

#pragma warning disable CA1416
public sealed class ServiceControllerWrapper(string serviceName) : IServiceControllerWrapper
{
    private readonly ServiceController _serviceController = new(serviceName);

    public ServiceControllerStatus Status => _serviceController.Status;

    public void Start() => _serviceController.Start();

    public void Stop() => _serviceController.Stop();

    public void WaitForStatus(ServiceControllerStatus status, TimeSpan timeout) => _serviceController.WaitForStatus(status, timeout);
}
