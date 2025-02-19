namespace Sentinel.WorkerService.Core.Windows.SecurityScan.Enums;

public enum ScanType
{
    Default = 0, // according to your configuration
    Quick = 1, // Quick scan
    Full = 2, // Full system scan
    Custom = 3 // File and directory custom scan
}