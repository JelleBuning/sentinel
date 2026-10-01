using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.Ping;
using Mycelium.WorkerService.Core.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.Ping;

public class PingModuleTests
{
    [Test]
    public async Task Execute_PingsTheApi()
    {
        var apiService = MyceliumApiServiceFactory.CreateSucceeding(out var handler);
        var config = Substitute.For<IScheduledModuleConfig<PingModule>>();
        var module = new PingModule(NullLogger<PingModule>.Instance, config, apiService);

        await module.Execute(CancellationToken.None);

        Assert.That(handler.Requests, Has.Count.EqualTo(1));
        Assert.That(handler.Requests[0].RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/1/ping"));
    }
}
