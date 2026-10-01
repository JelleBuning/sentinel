using Mycelium.WorkerService.Common.Helpers;

namespace Mycelium.WorkerService.Common.Hardware;

public sealed class MemoryInfoProvider : IMemoryInfoProvider
{
    public long GetInstalledMemoryKilobytes()
    {
        Kernel32Helper.GetPhysicallyInstalledSystemMemory(out var kilobytes);
        return kilobytes;
    }
}
