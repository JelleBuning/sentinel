using Microsoft.AspNetCore.SignalR.Client;
using Mycelium.WorkerService.Common.Consumer.Interfaces;
using NSubstitute;

namespace Mycelium.WorkerService.Core.UnitTests.TestSupport;

internal static class ConsumerConfigFactory
{
    /// <summary>
    /// ConsumerBase's constructor requires a non-null Connection to register its "On" handler.
    /// Building a real HubConnection performs no network I/O until StartAsync is called, so this
    /// is safe to use in a unit test.
    /// </summary>
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
