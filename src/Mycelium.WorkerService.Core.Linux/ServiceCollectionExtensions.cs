using Microsoft.Extensions.DependencyInjection;
using Mycelium.WorkerService.Core.Linux.SecurityScan;
using Mycelium.WorkerService.Core.Linux.TimeSync;
using Mycelium.WorkerService.Core.SecurityScan;
using Mycelium.WorkerService.Core.TimeSync;
using Mycelium.WorkerService.RemoteAccess.Services;
using Mycelium.WorkerService.RemoteAccess.Services.Interfaces;

namespace Mycelium.WorkerService.Core.Linux;

public static class ServiceCollectionExtensions
{
    public static void AddLinuxCoreDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IRemoteAccessService, LinuxRemoteAccess>();
        services.AddSingleton<ISecurityScanner, LinuxSecurity>();
        services.AddSingleton<ITimeSynchronizer, LinuxTimeSync>();

    }
}