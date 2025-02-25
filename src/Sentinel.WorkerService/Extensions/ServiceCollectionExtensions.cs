using Microsoft.AspNetCore.SignalR.Client;
using Sentinel.Common.Messages;
using Sentinel.WorkerService.Common.Consumer;
using Sentinel.WorkerService.Common.Consumer.Interfaces;
using Sentinel.WorkerService.Common.Module;
using Sentinel.WorkerService.Common.Module.Interfaces;
using Sentinel.WorkerService.Core.DeviceInformation;
using Sentinel.WorkerService.Core.Linux;
using Sentinel.WorkerService.Core.Ping;
using Sentinel.WorkerService.Core.RestartDevice;
using Sentinel.WorkerService.Core.SecurityScan;
using Sentinel.WorkerService.Core.TimeSync;
using Sentinel.WorkerService.Core.Windows;
using Sentinel.WorkerService.RemoteAccess;
using Sentinel.WorkerService.Services;

namespace Sentinel.WorkerService.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddModuleDependencies(this IServiceCollection services)
    {
        if (OperatingSystem.IsWindows())
        {
            services.AddWindowsCoreDependencies();
        }
        else if (OperatingSystem.IsLinux())
        {
            services.AddLinuxCoreDependencies();
        }
        else throw new Exception("This OS is not supported");
    }

    public static IServiceCollection AddStartupModules(this IServiceCollection services)
    {
        services
            .AddStartupTask<TimeSyncModule>()
            .AddStartupTask<PlaceholderModule>();
        return services;
    }

    public static IServiceCollection AddScheduledModules(this IServiceCollection services)
    {
        services.AddScheduledTask<PingModule>(scheduleConfig => scheduleConfig.Interval = TimeSpan.FromMinutes(0.75))
            .AddScheduledTask<DeviceInformationModule>(scheduleConfig =>
                scheduleConfig.Interval = TimeSpan.FromMinutes(5))
            .AddScheduledTask<StorageInformationModule>(scheduleConfig =>
                scheduleConfig.Interval = TimeSpan.FromMinutes(10))
            .AddScheduledTask<SecurityInformationModule>(scheduleConfig =>
                scheduleConfig.Interval = TimeSpan.FromMinutes(2.5))
            .AddScheduledTask<SoftwareInformationModule>(scheduleConfig =>
                scheduleConfig.Interval = TimeSpan.FromMinutes(30));
        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services, HubConnection hubConnection)
    {
        services
            .AddConsumer<RestartDeviceModule, RestartDeviceMessage>(config => config.Connection = hubConnection)
            .AddConsumer<RemoteAccessModule, RemoteAccessMessage>(config => config.Connection = hubConnection)
            .AddConsumer<SecurityScanModule, SecurityScanMessage>(config => config.Connection = hubConnection);

        return services;
    }

    private static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
        where T : class, IStartupModule, IModule
    {
        if (!LicenseManager.IsLicensed<T>()) return services;

        services.AddSingleton<IStartupModule, T>();
        return services;
    }

    private static IServiceCollection AddConsumer<TConsumer, TMessage>(this IServiceCollection services,
        Action<IConsumerConfig<TMessage>> consumerConfig) where TConsumer : class, IHostedService, IModule
    {
        if (consumerConfig == null)
            throw new ArgumentNullException(nameof(consumerConfig), "Please provide consumer configuration");
        if (!LicenseManager.IsLicensed<TConsumer>()) return services;

        var config = new ConsumerConfig<TMessage>();
        consumerConfig(config);

        services.AddSingleton<IConsumerConfig<TMessage>>(config);
        services.AddHostedService<TConsumer>();
        return services;
    }

    private static IServiceCollection AddScheduledTask<T>(this IServiceCollection services,
        Action<IScheduledModuleConfig<T>> scheduledTaskConfig) where T : ScheduledModuleBase<T>, IModule
    {
        if (scheduledTaskConfig == null)
            throw new ArgumentNullException(nameof(scheduledTaskConfig), "Please provide scheduled task configuration");
        if (!LicenseManager.IsLicensed<T>()) return services;

        var config = new ScheduledModuleConfig<T>();
        scheduledTaskConfig.Invoke(config);

        services.AddSingleton<IScheduledModuleConfig<T>>(config);
        services.AddHostedService<T>();

        return services;
    }

    public static void Build(this IServiceCollection _)
    {
    }
}