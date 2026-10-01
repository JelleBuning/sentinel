using Mycelium.WorkerService.Core.Linux.SecurityScan;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Linux.UnitTests.SecurityScan;

public class LinuxSecurityTests
{
    [Test]
    public void Scan_ThrowsNotImplementedException()
    {
        var linuxSecurity = new LinuxSecurity();

        Assert.ThrowsAsync<NotImplementedException>(() => linuxSecurity.Scan(CancellationToken.None));
    }
}
