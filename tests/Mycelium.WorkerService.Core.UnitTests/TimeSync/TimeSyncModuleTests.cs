using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.WorkerService.Core.TimeSync;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.TimeSync;

public class TimeSyncModuleTests
{
    [Test]
    public async Task Execute_SynchronizesTime()
    {
        var synchronizer = Substitute.For<ITimeSynchronizer>();
        synchronizer.Synchronize().Returns(Task.CompletedTask);
        var module = new TimeSyncModule(synchronizer, NullLogger<TimeSyncModule>.Instance);

        await module.Execute(CancellationToken.None);

        await synchronizer.Received(1).Synchronize();
    }
}

public class PlaceholderModuleTests
{
    [Test]
    public void Execute_ThrowsNotImplemented()
    {
        var module = new PlaceholderModule();

        Assert.ThrowsAsync<NotImplementedException>(async () => await module.Execute(CancellationToken.None));
    }
}
