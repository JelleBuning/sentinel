using Microsoft.Extensions.DependencyInjection;
using Sentinel.WorkerService.Core.Linux.SecurityScan;
using Sentinel.WorkerService.Core.Linux.TimeSync;
using Sentinel.WorkerService.Core.SecurityScan;
using Sentinel.WorkerService.Core.TimeSync;
using Sentinel.WorkerService.RemoteAccess.Services;
using Sentinel.WorkerService.RemoteAccess.Services.Interfaces;

namespace Sentinel.WorkerService.Core.Linux;

public static class ServiceCollectionExtensions
{
    public static void AddLinuxCoreDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IRemoteAccessService, LinuxRemoteAccess>();
        services.AddSingleton<ISecurityScanner, LinuxSecurity>();
        services.AddSingleton<ITimeSynchronizer, LinuxTimeSync>();

    }
}