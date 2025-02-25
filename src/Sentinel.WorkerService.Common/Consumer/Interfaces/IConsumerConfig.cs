using Microsoft.AspNetCore.SignalR.Client;

namespace Sentinel.WorkerService.Common.Consumer.Interfaces;

// ReSharper disable once UnusedTypeParameter
public interface IConsumerConfig<T>
{
    HubConnection? Connection { get; set; }
}