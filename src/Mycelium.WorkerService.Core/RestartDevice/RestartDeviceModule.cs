using Microsoft.Extensions.Logging;
using Mycelium.Common.SignalR;
using Mycelium.WorkerService.Common.Consumer;
using Mycelium.WorkerService.Common.Consumer.Interfaces;

namespace Mycelium.WorkerService.Core.RestartDevice;

public class RestartDeviceModule(IConsumerConfig<RestartDeviceMessage> config, ILogger<RestartDeviceMessage> logger) : ConsumerBase<RestartDeviceMessage, bool>(config, logger)
{
    protected override Task<bool> OnMessageReceived(RestartDeviceMessage context)
    {
        try
        {
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            return Task.FromException<bool>(ex);
        }
    }
}