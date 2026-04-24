using Mycelium.WorkerService.Core.SecurityScan;

namespace Mycelium.WorkerService.Core.Linux.SecurityScan;

public class LinuxSecurity : ISecurityScanner
{
    public Task<bool> Scan(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}