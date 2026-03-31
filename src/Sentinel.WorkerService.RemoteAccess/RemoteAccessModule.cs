using Microsoft.Extensions.Logging;
using Sentinel.Common.SignalR;
using Sentinel.WorkerService.Common.Consumer;
using Sentinel.WorkerService.Common.Consumer.Interfaces;
using Sentinel.WorkerService.RemoteAccess.Services.Interfaces;

namespace Sentinel.WorkerService.RemoteAccess;

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