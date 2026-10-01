using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.Wmi;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public sealed class DiskInformationRetriever(IWmiQueryService wmiQueryService) : IDiskInformationRetriever
{
    public List<DiskDto> Retrieve()
    {
        var osDir = Path.GetPathRoot(Environment.SystemDirectory);
        var health = GetHealthByDriveLetter();

        return DriveInfo.GetDrives()
            .Where(x => x.IsReady)
            .Select(x => MapToDiskDto(x.Name, x.TotalSize, x.TotalSize - x.TotalFreeSpace, osDir == x.Name, health))
            .ToList();
    }

    internal static DiskDto MapToDiskDto(string driveName, double size, double used, bool isOsDisk, Dictionary<string, string?> health)
    {
        var name = driveName.TrimEnd('\\');
        health.TryGetValue(name, out var status);

        return new DiskDto
        {
            Name = name,
            Size = size,
            Used = used,
            IsOsDisk = isOsDisk,
            HealthStatus = status
        };
    }

    internal Dictionary<string, string?> GetHealthByDriveLetter()
    {
        var result = new Dictionary<string, string?>();

        try
        {
            var diskDrives = wmiQueryService.Query("SELECT * FROM Win32_DiskDrive");
            foreach (var diskDrive in diskDrives)
            {
                try
                {
                    var deviceId = diskDrive.TryGetValue("DeviceID", out var deviceIdValue) ? deviceIdValue?.ToString() : null;
                    var status = NormalizeStatus(diskDrive.TryGetValue("Status", out var statusValue) ? statusValue?.ToString() : null);
                    if (deviceId is null)
                    {
                        continue;
                    }

                    foreach (var driveLetter in GetDriveLettersForDisk(deviceId))
                    {
                        result[driveLetter] = status;
                    }
                }
                catch
                {
                }
            }
        }
        catch
        {
        }

        return result;
    }

    internal static string? NormalizeStatus(string? wmiStatus)
    {
        return wmiStatus switch
        {
            null => null,
            "OK" => DiskHealthStatus.Ok,
            "Degraded" or "Stressed" => DiskHealthStatus.Degraded,
            "Pred Fail" or "Error" or "NonRecover" => DiskHealthStatus.Failing,
            _ => DiskHealthStatus.Unknown
        };
    }

    internal IEnumerable<string> GetDriveLettersForDisk(string diskDeviceId)
    {
        var partitions = wmiQueryService.Query(
            $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{diskDeviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");

        foreach (var partition in partitions)
        {
            var partitionDeviceId = partition.TryGetValue("DeviceID", out var partitionDeviceIdValue) ? partitionDeviceIdValue?.ToString() : null;

            var logicalDisks = wmiQueryService.Query(
                $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionDeviceId}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");

            foreach (var logicalDisk in logicalDisks)
            {
                var driveLetter = logicalDisk.TryGetValue("DeviceID", out var driveLetterValue) ? driveLetterValue?.ToString() : null;
                if (driveLetter is not null)
                {
                    yield return driveLetter;
                }
            }
        }
    }
}
