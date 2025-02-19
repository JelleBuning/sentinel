using System.Runtime.InteropServices;

namespace Sentinel.WorkerService.Common.Helpers;

public static class Kernel32Helper
{
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);

}