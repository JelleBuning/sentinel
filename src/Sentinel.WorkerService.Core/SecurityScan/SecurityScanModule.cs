using Microsoft.Extensions.Logging;
using Sentinel.Common.SignalR;
using Sentinel.WorkerService.Common.Consumer;
using Sentinel.WorkerService.Common.Consumer.Interfaces;

namespace Sentinel.WorkerService.Core.SecurityScan;

public class SecurityScanModule(IConsumerConfig<SecurityScanMessage> config, ILogger<SecurityScanMessage> logger, ISecurityScanner scanner) : ConsumerBase<SecurityScanMessage, bool>(config, logger)
{
    protected override async Task<bool> OnMessageReceived(SecurityScanMessage context)
    {
        try
        {
            return await scanner.Scan(CancellationToken.None);
        }
        catch (Exception ex)
        {
            return await Task.FromException<bool>(ex);
        }
    }
}