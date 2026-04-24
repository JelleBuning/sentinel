using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

public class StorageInformationRetriever : IStorageInformationRetriever
{
    public StorageInformationDto Retrieve()
    {
        var osDir = Path.GetPathRoot(Environment.SystemDirectory);
        return new StorageInformationDto
        {
            Disks = DriveInfo.GetDrives().Select(x => new DiskInformationDto()
            {
                Name = x.Name.TrimEnd('\\'), Size = x.TotalSize, Used = x.TotalSize - x.TotalFreeSpace,
                IsOsDisk = osDir == x.Name
            }).ToList()
        };
    }
}