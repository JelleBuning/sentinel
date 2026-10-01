using Mycelium.WorkerService.RemoteAccess.Services;
using NUnit.Framework;

namespace Mycelium.WorkerService.RemoteAccess.UnitTests.Services;

public class LinuxRemoteAccessTests
{
    [Test]
    public void Start_ThrowsNotImplemented()
    {
        var service = new LinuxRemoteAccess();

        Assert.Throws<NotImplementedException>(() => service.Start());
    }

    [Test]
    public void Stop_ThrowsNotImplemented()
    {
        var service = new LinuxRemoteAccess();

        Assert.Throws<NotImplementedException>(() => service.Stop());
    }
}
