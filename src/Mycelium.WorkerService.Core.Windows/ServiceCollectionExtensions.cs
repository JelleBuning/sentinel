using Microsoft.Extensions.DependencyInjection;
using Mycelium.WorkerService.Common.Hardware;
using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.SecurityScan;
using Mycelium.WorkerService.Core.TimeSync;
using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using Mycelium.WorkerService.Core.Windows.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.Registry;
using Mycelium.WorkerService.Core.Windows.SecurityScan;
using Mycelium.WorkerService.Core.Windows.TimeSync;
using Mycelium.WorkerService.Core.Windows.Wmi;
using Mycelium.WorkerService.RemoteAccess.Services;
using Mycelium.WorkerService.RemoteAccess.Services.Interfaces;

namespace Mycelium.WorkerService.Core.Windows;

public static class ServiceCollectionExtensions
{
    public static void AddWindowsCoreDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IWmiQueryService, WmiQueryService>();
        services.AddSingleton<IRegistryService, RegistryService>();
        services.AddSingleton<IMemoryInfoProvider, MemoryInfoProvider>();
        services.AddSingleton<IProcessRunner, ProcessRunner>();

        services.AddSingleton<IServiceControllerWrapper>(new ServiceControllerWrapper("AnyDesk Service"));
        services.AddSingleton<IRemoteAccessService, AnyDeskService>();
        services.AddSingleton<ISecurityScanner, WinDefenderService>();
        services.AddSingleton<ITimeSynchronizer, TimeSynchronizer>();

        services.AddSingleton<IDeviceInformationRetriever, DeviceInformationRetriever>();
        services.AddSingleton<IDiskInformationRetriever, DiskInformationRetriever>();
        services.AddSingleton<IFirewallSettingsRetriever, FirewallSettingsRetriever>();
        services.AddSingleton<ISecurityInformationRetriever, SecurityInformationRetriever>();
        services.AddSingleton<ISoftwareInformationRetriever, SoftwareInformationRetriever>();
    }
}