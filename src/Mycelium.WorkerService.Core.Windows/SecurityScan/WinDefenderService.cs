using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.Core.SecurityScan;
using Mycelium.WorkerService.Core.Windows.SecurityScan.Enums;

namespace Mycelium.WorkerService.Core.Windows.SecurityScan;

public class WinDefenderService : ISecurityScanner
{
    private readonly IProcessRunner _processRunner;
    private bool _isDefenderAvailable;
    private readonly string? _defenderPath;
    private readonly SemaphoreSlim _lock = new(3); //limit to 3 concurrent checks at a time

    public WinDefenderService(IProcessRunner processRunner)
    {
        _processRunner = processRunner;
        _defenderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Windows Defender", "MpCmdRun.exe");
        _isDefenderAvailable = File.Exists(_defenderPath);
    }

    internal WinDefenderService(IProcessRunner processRunner, string? defenderPath, bool isDefenderAvailable)
    {
        _processRunner = processRunner;
        _defenderPath = defenderPath;
        _isDefenderAvailable = isDefenderAvailable;
    }

    public async Task<bool> Scan(CancellationToken cancellationToken)
    {
        if (!_isDefenderAvailable) return false;
        if (_defenderPath == null) return false;
        await _lock.WaitAsync(cancellationToken);

        try
        {
            using var handle = _processRunner.Start(_defenderPath, $"-Scan -ScanType {(int)ScanType.Quick}");
            _ = Task.Run(handle.ReadToEnd, CancellationToken.None);

            await handle.WaitForExitAsync(cancellationToken)
                .WaitAsync(TimeSpan.FromMilliseconds(2500), cancellationToken);
            return handle.ExitCode == 2;
        }
        finally
        {
            _lock.Release();
        }
    }
}
