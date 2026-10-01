using Mycelium.WorkerService.Core.Linux.TimeSync;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Linux.UnitTests.TimeSync;

public class LinuxTimeSyncTests
{
    [Test]
    public void Synchronize_ThrowsNotImplementedException()
    {
        var linuxTimeSync = new LinuxTimeSync();

        Assert.ThrowsAsync<NotImplementedException>(() => linuxTimeSync.Synchronize());
    }
}
