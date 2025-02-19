using Serilog;
using Serilog.Events;

namespace Sentinel.WorkerService.Extensions;

public static class SerilogExtensions
{
    public static void AddSerilogConsole()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
    }

    public static void AddSerilogEventLog()
    {
#pragma warning disable CA1416
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.EventLog("Sentinel workerservice")
            .CreateLogger();
#pragma warning restore CA1416
    }
}