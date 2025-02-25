using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentinel.WorkerService.Common.Consumer.Interfaces;
using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Common.Consumer;

public abstract class ConsumerBase<TMessage, TResponse> : IHostedService, IModule
{
    public abstract Task<TResponse> OnMessageReceived(TMessage context);

    protected ConsumerBase(IConsumerConfig<TMessage> config, ILogger<TMessage> logger)
    {
        if(config.Connection == null) throw new ArgumentNullException(nameof(config.Connection));
        var messageName = typeof(TMessage).Name;
        
        config.Connection.On(messageName, (TMessage message) =>
        {
            logger.LogInformation($"[*] {messageName} received");
            OnMessageReceived(message);
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}