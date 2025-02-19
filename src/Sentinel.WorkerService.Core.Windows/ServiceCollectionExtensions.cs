using Microsoft.Extensions.DependencyInjection;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;
using Sentinel.WorkerService.Core.SecurityScan;
using Sentinel.WorkerService.Core.TimeSync;
using Sentinel.WorkerService.Core.Windows.DeviceInformation;
using Sentinel.WorkerService.Core.Windows.DeviceInformation.Interfaces;
using Sentinel.WorkerService.Core.Windows.SecurityScan;
using Sentinel.WorkerService.Core.Windows.TimeSync;
using Sentinel.WorkerService.RemoteAccess.Services;
using Sentinel.WorkerService.RemoteAccess.Services.Interfaces;

namespace Sentinel.WorkerService.Core.Windows;

public static class ServiceCollectionExtensions
{
    public static void AddWindowsCoreDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IRemoteAccessService, AnyDeskService>();
        services.AddSingleton<ISecurityScanner, WinDefenderService>();
        services.AddSingleton<ITimeSynchronizer, TimeSynchronizer>();
        
        services.AddSingleton<IDeviceInformationRetriever, DeviceInformationRetriever>();
        services.AddSingleton<IStorageInformationRetriever, StorageInformationRetriever>();
        services.AddSingleton<IFirewallSettingsRetriever, FirewallSettingsRetriever>();
        services.AddSingleton<ISecurityInformationRetriever, SecurityInformationRetriever>();
        services.AddSingleton<ISoftwareInformationRetriever, SoftwareInformationRetriever>();
    }
}