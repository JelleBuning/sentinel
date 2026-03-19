using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Sentinel.Api;
using Sentinel.Api.Application;
using Sentinel.Api.Extensions;
using Sentinel.Api.Infrastructure;
using Sentinel.Api.Infrastructure.Middleware;
using Sentinel.Api.Infrastructure.Persistence;
using Sentinel.Api.Infrastructure.SignalR;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Services
            .AddPresentation()
            .AddApplication()
            .AddInfrastructure(builder.Configuration);
    }
    
    Log.Information("Starting host");
    var app = builder.Build();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }
    
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    
    app.AddHub<DeviceMessageHub>();

    app.UseCors(corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    using (var scope = app.Services.CreateScope())
    {
        // It skips migration in test, non-relational DB
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (dbContext.Database.IsRelational()) dbContext.Database.Migrate();
    }
    await app.RunAsync();
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

namespace Sentinel.Api
{
    public abstract class Program;
}