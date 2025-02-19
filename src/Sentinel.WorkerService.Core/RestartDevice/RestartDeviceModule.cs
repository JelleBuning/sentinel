using Microsoft.Extensions.Logging;
using Sentinel.Common.Messages;
using Sentinel.WorkerService.Common.Consumer;
using Sentinel.WorkerService.Common.Consumer.Interfaces;

namespace Sentinel.WorkerService.Core.RestartDevice;

public class RestartDeviceModule(IConsumerConfig<RestartDeviceMessage> config, ILogger<RestartDeviceMessage> logger) : ConsumerBase<RestartDeviceMessage, bool>(config, logger)
{
    public override Task<bool> OnMessageReceived(RestartDeviceMessage context)
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