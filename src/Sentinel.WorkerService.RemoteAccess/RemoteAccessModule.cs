using Microsoft.Extensions.Logging;
using Sentinel.Common.Messages;
using Sentinel.WorkerService.Common.Consumer;
using Sentinel.WorkerService.Common.Consumer.Interfaces;
using Sentinel.WorkerService.RemoteAccess.Services.Interfaces;

namespace Sentinel.WorkerService.RemoteAccess;

public class RemoteAccessModule(IConsumerConfig<RemoteAccessMessage> config, ILogger<RemoteAccessMessage> logger, IRemoteAccessService remoteAccessService) : ConsumerBase<RemoteAccessMessage, RemoteAccessResponseMessage>(config, logger)
{
    public override Task<RemoteAccessResponseMessage> OnMessageReceived(RemoteAccessMessage context)
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