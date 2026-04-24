using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Consumer.Interfaces;
using Mycelium.WorkerService.Common.Module.Interfaces;

namespace Mycelium.WorkerService.Common.Consumer;

public abstract class ConsumerBase<TMessage, TResponse> : IHostedService, IModule
{
    protected abstract Task<TResponse> OnMessageReceived(TMessage context);

    protected ConsumerBase(IConsumerConfig<TMessage> config, ILogger<TMessage> logger)
    {
        if(config.Connection == null) throw new ArgumentNullException(nameof(config.Connection));
        var messageName = typeof(TMessage).Name;
        
        config.Connection.On(messageName, (TMessage message) =>
        {
            logger.LogInformation("[*] {MessageName} received", messageName);
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