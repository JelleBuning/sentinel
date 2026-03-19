using Microsoft.Extensions.Logging;
using Sentinel.Common.SignalR;
using Sentinel.WorkerService.Common.Consumer;
using Sentinel.WorkerService.Common.Consumer.Interfaces;

namespace Sentinel.WorkerService.Core.RestartDevice;

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