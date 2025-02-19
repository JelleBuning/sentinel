using Microsoft.AspNetCore.SignalR.Client;
using Sentinel.WorkerService.Common.Consumer.Interfaces;

namespace Sentinel.WorkerService.Common.Consumer;

public class ConsumerConfig<T> : IConsumerConfig<T>
{
    public HubConnection Connection { get; set; }
}