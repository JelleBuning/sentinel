using Microsoft.AspNetCore.SignalR.Client;

namespace Sentinel.WorkerService.Common.Consumer.Interfaces;

public interface IConsumerConfig<T>
{
    HubConnection? Connection { get; set; }
}