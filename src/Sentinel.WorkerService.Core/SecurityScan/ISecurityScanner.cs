namespace Sentinel.WorkerService.Core.SecurityScan;

public interface ISecurityScanner
{
    public Task<bool> Scan(CancellationToken cancellationToken);
}