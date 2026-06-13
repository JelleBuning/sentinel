using Serilog;
using System.Net.Http.Headers;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.Api.Interfaces;
using Mycelium.WorkerService.Common.Services;
using Mycelium.WorkerService.Common.Services.Interfaces;
using Mycelium.WorkerService.Extensions;
using Mycelium.WorkerService.Services;

try
{
#if DEBUG
    SerilogExtensions.AddSerilogConsole();
#else
    SerilogExtensions.AddSerilogEventLog();
#endif

    Log.Warning("Starting service");
    var host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
        {
            // Add dependencies
            services.AddTransient<ICredentialManager, CredentialManager>();
            services.AddTransient<IAuthenticationHandler, AuthenticationHandler>();
            services.AddTransient<AuthenticationDelegatingHandler>();
            
            // Add HttpClient
            services.AddHttpClient<MyceliumApiService>(client =>
            {
                client.BaseAddress = new Uri(hostContext.Configuration.GetConnectionString("Api") ?? throw new Exception("Api configuration not found."));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

            // Build the service provider to resolve the authentication handler
            var authenticationHandler = services.BuildServiceProvider().GetRequiredService<IAuthenticationHandler>();
            authenticationHandler.EnsureAuthenticatedAsync(Guid.Parse(args[0]), Environment.MachineName, CancellationToken.None).Wait();
            
            // SignalR
            var deviceHubConnection = HubManager.Initialize("DeviceMessageHub", hostContext);
            
            services.AddModuleDependencies();
            services
                .AddStartupModules()
                .AddScheduledModules()
                .AddConsumers(deviceHubConnection)
                .Build();

            _ = Task.Run(async () => await HubManager.Connect(deviceHubConnection)); // Don't block startup
        })
        .UseSerilog()
        .UseWindowsService()
        .UseSystemd()
        .Build();

    host.ExecuteStartupModules();
    await host.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}