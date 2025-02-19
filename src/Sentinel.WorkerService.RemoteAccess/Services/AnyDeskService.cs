using System.ServiceProcess;
using Sentinel.WorkerService.Common.Helpers;
using Sentinel.WorkerService.RemoteAccess.Models;
using Sentinel.WorkerService.RemoteAccess.Services.Interfaces;

namespace Sentinel.WorkerService.RemoteAccess.Services;

#pragma warning disable CA1416
public class AnyDeskService : IRemoteAccessService
{
    private const string ExecutablePath = @"C:\Program Files (x86)\AnyDesk\AnyDesk.exe";
    private const string PowershellExe = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe";
    private readonly ServiceController _serviceController = new("AnyDesk Service");

    public bool IsRunning => _serviceController.Status == ServiceControllerStatus.Running;

    public ConnectionDetails Start()
    {
        if(_serviceController.Status != ServiceControllerStatus.Running) _serviceController.Start();
        _serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(7.5));

        var idProcess = ProcessHelper.Start(PowershellExe, $"&'{ExecutablePath}'  --get-id | ForEach-Object {{ Write-Host $_ }}");
        var id = idProcess.StandardOutput.ReadLine() ?? throw new Exception("WindowsRemoteAccess id not found");
        _ = EnsureProcessDisposes();

        return new ConnectionDetails
        {
            Id = id,
        };

    }

    public void Stop()
    {
        if (_serviceController.Status != ServiceControllerStatus.Running) return;
        _serviceController.Stop();
        _serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(7.5));
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