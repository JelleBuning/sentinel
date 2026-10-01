using System.ServiceProcess;
using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.RemoteAccess.Models;
using Mycelium.WorkerService.RemoteAccess.Services.Interfaces;

namespace Mycelium.WorkerService.RemoteAccess.Services;

#pragma warning disable CA1416
public class AnyDeskService(IProcessRunner processRunner, IServiceControllerWrapper serviceController) : IRemoteAccessService
{
    private const string ExecutablePath = @"C:\Program Files (x86)\AnyDesk\AnyDesk.exe";
    private const string PowershellExe = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe";

    public bool IsRunning => serviceController.Status == ServiceControllerStatus.Running;

    public ConnectionDetails Start()
    {
        if(serviceController.Status != ServiceControllerStatus.Running) serviceController.Start();
        serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(7.5));

        var idProcess = processRunner.Start(PowershellExe, $"&'{ExecutablePath}'  --get-id | ForEach-Object {{ Write-Host $_ }}");
        var id = idProcess.ReadLine() ?? throw new Exception("WindowsRemoteAccess id not found");
        _ = EnsureProcessDisposes();

        return new ConnectionDetails
        {
            Id = id,
        };

    }

    public void Stop()
    {
        if (serviceController.Status != ServiceControllerStatus.Running) return;
        serviceController.Stop();
        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(7.5));
    }

    private async Task EnsureProcessDisposes()
    {
        await new Task(() =>
        {
            // TODO: Get tcp connections on _serviceController
            // TODO: When inactive close and stop service
            Stop();
        }).WaitAsync(TimeSpan.FromSeconds(5));
    }
}
