using Microsoft.AspNetCore.SignalR.Client;
using Mycelium.WorkerService.Common.Consumer.Interfaces;
using NSubstitute;

namespace Mycelium.WorkerService.RemoteAccess.UnitTests.TestSupport;

internal static class ConsumerConfigFactory
{
    public static IConsumerConfig<T> Create<T>()
    {
        var config = Substitute.For<IConsumerConfig<T>>();
        config.Connection.Returns(new HubConnectionBuilder().WithUrl("http://localhost/hub").Build());
        return config;
    }

    public static object InvokeOnMessageReceived<TConsumer>(TConsumer consumer, object message)
    {
        var method = typeof(TConsumer).GetMethod("OnMessageReceived",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        return method.Invoke(consumer, [message])!;
    }
}
