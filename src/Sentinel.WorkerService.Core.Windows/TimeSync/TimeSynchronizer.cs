using Sentinel.WorkerService.Common.Helpers;
using Sentinel.WorkerService.Core.TimeSync;

namespace Sentinel.WorkerService.Core.Windows.TimeSync;

public class TimeSynchronizer : ITimeSynchronizer
{
    public Task Synchronize()
    {
        var psTimeScript = "Set-Service w32time -StartupType manual" + Environment.NewLine;
        psTimeScript += "Start-Service -Name 'w32time'" + Environment.NewLine;
        psTimeScript += "w32tm /config /syncfromflags:manual '/manualpeerlist:0.pool.ntp.org,0x1 1.pool.ntp.org,0x1 2.pool.ntp.org,0x1 3.pool.ntp.org,0x1' / reliable:yes" + Environment.NewLine;
        psTimeScript += "w32tm /config /update" + Environment.NewLine;
        psTimeScript += "w32tm /resync /force" + Environment.NewLine;
        psTimeScript += "Set-Service w32time -StartupType disabled" + Environment.NewLine;

        var process = ProcessHelper.Start("C:\\windows\\system32\\windowspowershell\\v1.0\\powershell.exe", psTimeScript);
        return process.WaitForExitAsync();
    }
}