using Microsoft.Extensions.Logging;
using Mycelium.Common.SignalR;
using Mycelium.WorkerService.Common.Consumer;
using Mycelium.WorkerService.Common.Consumer.Interfaces;
using Mycelium.WorkerService.RemoteAccess.Services.Interfaces;

namespace Mycelium.WorkerService.RemoteAccess;

public class RemoteAccessModule(IConsumerConfig<RemoteAccessMessage> config, ILogger<RemoteAccessMessage> logger, IRemoteAccessService remoteAccessService) : ConsumerBase<RemoteAccessMessage, RemoteAccessResponseMessage>(config, logger)
{
    protected override Task<RemoteAccessResponseMessage> OnMessageReceived(RemoteAccessMessage context)
    {
        try
        {
            var connectionDetails = remoteAccessService.Start();
            return Task.FromResult(new RemoteAccessResponseMessage(connectionDetails.Id));
        }
        catch (Exception ex)
        {
            return Task.FromException<RemoteAccessResponseMessage>(ex);
        }
    }
}