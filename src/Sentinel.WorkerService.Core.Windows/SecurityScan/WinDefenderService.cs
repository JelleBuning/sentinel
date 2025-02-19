using System.Diagnostics;
using Sentinel.WorkerService.Core.SecurityScan;
using Sentinel.WorkerService.Core.Windows.SecurityScan.Enums;

namespace Sentinel.WorkerService.Core.Windows.SecurityScan;

public class WinDefenderService : ISecurityScanner
{
    private bool _isDefenderAvailable;
    private readonly string? _defenderPath;
    private readonly SemaphoreSlim _lock = new(3); //limit to 3 concurrent checks at a time

    public WinDefenderService()
    {
        _defenderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Windows Defender", "MpCmdRun.exe");
        _isDefenderAvailable = File.Exists(_defenderPath);
    }

    public async Task<bool> Scan(CancellationToken cancellationToken)
    {
        if (!_isDefenderAvailable) return false;
        if (_defenderPath == null) return false;
        await _lock.WaitAsync(cancellationToken);

        try
        {
            using var process = Process.Start(_defenderPath, $"-Scan -ScanType {(int)ScanType.Quick}");
            if (process == null)
            {
                _isDefenderAvailable = false; //disable future attempts
                throw new InvalidOperationException("Failed to start MpCmdRun.exe");
            }

            await process.WaitForExitAsync(cancellationToken)
                .WaitAsync(TimeSpan.FromMilliseconds(2500), cancellationToken);
            return process.ExitCode == 2;
        }
        finally
        {
            _lock.Release();
        }
    }
}