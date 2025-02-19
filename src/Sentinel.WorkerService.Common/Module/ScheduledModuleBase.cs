using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Module.Interfaces;
using Timer = System.Timers.Timer;

namespace Sentinel.WorkerService.Common.Module
{
    public abstract class ScheduledModuleBase<T>(ILogger<T> logger, IScheduledModuleConfig<T> config, bool runImmediately = false) : IHostedService, IModule
    {
        public bool RunImmediately { get; set; } = runImmediately;
        private Timer? _timer;

        public string Name => typeof(T).Name;

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            if (RunImmediately)
            {
                await LogExecute(cancellationToken);
            }
            else
            {
                await Schedule(cancellationToken);
            }
        }

        public abstract Task Execute(CancellationToken cancellationToken);


        protected virtual async Task Schedule(CancellationToken cancellationToken)
        {
            if (config.Interval.TotalMilliseconds <= 0)
            {
                await Schedule(cancellationToken);
            }

            _timer = new Timer(config.Interval.TotalMilliseconds);
            _timer.Elapsed += async (_, _) =>
            {
                _timer.Dispose();
                _timer = null;
                if (cancellationToken.IsCancellationRequested) return;

                await LogExecute(cancellationToken);
            };
            _timer.Start();

            await Task.CompletedTask;
        }

        private async Task LogExecute(CancellationToken cancellationToken)
        {
            logger.LogInformation($"[{Name}] Executing");
            try
            {
                await Execute(cancellationToken);
                logger.LogInformation($"[{Name}] Executed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Name}] Failed: \"{ex.Message}\"");
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    await Schedule(cancellationToken);
                }
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Stopping {Name} task");
            _timer?.Stop();
            await Task.CompletedTask;
        }
    }
}