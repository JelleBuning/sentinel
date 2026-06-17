using Microsoft.AspNetCore.SignalR.Client;

namespace Mycelium.WorkerService.Common.Consumer.Interfaces;

public interface IConsumerConfig<T>
{
    HubConnection? Connection { get; set; }
}