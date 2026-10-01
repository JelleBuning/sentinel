namespace Mycelium.WorkerService.Common.Hardware;

public interface IMemoryInfoProvider
{
    long GetInstalledMemoryKilobytes();
}
