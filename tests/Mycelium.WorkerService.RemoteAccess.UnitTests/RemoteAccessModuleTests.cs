using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.SignalR;
using Mycelium.WorkerService.RemoteAccess.Models;
using Mycelium.WorkerService.RemoteAccess.Services.Interfaces;
using Mycelium.WorkerService.RemoteAccess.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.RemoteAccess.UnitTests;

public class RemoteAccessModuleTests
{
    [Test]
    public async Task OnMessageReceived_StartsRemoteAccessAndReturnsConnectionId()
    {
        var remoteAccessService = Substitute.For<IRemoteAccessService>();
        remoteAccessService.Start().Returns(new ConnectionDetails { Id = "connection-1" });
        var config = ConsumerConfigFactory.Create<RemoteAccessMessage>();
        var module = new RemoteAccessModule(config, NullLogger<RemoteAccessMessage>.Instance, remoteAccessService);

        var resultTask = (Task<RemoteAccessResponseMessage>)ConsumerConfigFactory.InvokeOnMessageReceived(module, new RemoteAccessMessage());

        var result = await resultTask;
        Assert.That(result.ConnectionId, Is.EqualTo("connection-1"));
    }

    [Test]
    public void OnMessageReceived_StartThrows_ReturnsFaultedTask()
    {
        var remoteAccessService = Substitute.For<IRemoteAccessService>();
        remoteAccessService.Start().Returns(_ => throw new InvalidOperationException("boom"));
        var config = ConsumerConfigFactory.Create<RemoteAccessMessage>();
        var module = new RemoteAccessModule(config, NullLogger<RemoteAccessMessage>.Instance, remoteAccessService);

        var resultTask = (Task<RemoteAccessResponseMessage>)ConsumerConfigFactory.InvokeOnMessageReceived(module, new RemoteAccessMessage());

        Assert.That(resultTask.IsFaulted, Is.True);
    }
}
