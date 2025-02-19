using Sentinel.WorkerService.Core.SecurityScan;

namespace Sentinel.WorkerService.Core.Linux.SecurityScan;

public class LinuxSecurity : ISecurityScanner
{
    public Task<bool> Scan(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}