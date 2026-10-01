using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Auth;
using Mycelium.Api.Core;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Devices;
using Mycelium.Api.Devices.SignalR;
using Mycelium.Api.EntityFramework;
using Mycelium.Api.EntityFramework.Persistence;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

    builder.Services.Configure<JsonOptions>(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

    builder.Services
        .AddApiCore()
        .AddPersistence(builder.Configuration)
        .AddAuth(builder.Configuration)
        .AddDevicesFeature()
        .AddHttpContextAccessor()
        .AddProblemDetails();

    builder.Services.AddCors();

    Log.Information("Starting host");
    var app = builder.Build();

    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.UseExceptionHandler();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapEndpoints();
    app.MapDeviceMessageHub();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (dbContext.Database.IsRelational()) dbContext.Database.Migrate();
    }

    await app.RunAsync();
    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

namespace Mycelium.Api
{
    public abstract class Program;
}
