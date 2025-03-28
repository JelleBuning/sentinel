using Sentinel.Common.DTO.Device;
using Sentinel.WorkerService.Core.DeviceInformation.Interfaces;

namespace Sentinel.WorkerService.Core.Windows.DeviceInformation;

public class StorageInformationRetriever : IStorageInformationRetriever
{
    public StorageInformation Retrieve()
    {
        var osDir = Path.GetPathRoot(Environment.SystemDirectory);
        return new StorageInformation
        {
            Disks = DriveInfo.GetDrives().Select(x => new DiskInformation()
            {
                Name = x.Name.TrimEnd('\\'), Size = x.TotalSize, Used = x.TotalSize - x.TotalFreeSpace,
                IsOsDisk = osDir == x.Name
            }).ToList()
        };
    }
}