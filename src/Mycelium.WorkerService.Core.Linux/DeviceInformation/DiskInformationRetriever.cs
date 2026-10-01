using System.Text.Json;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.Linux.DeviceInformation;

public sealed class DiskInformationRetriever(IProcessRunner processRunner) : IDiskInformationRetriever
{
    public List<DiskDto> Retrieve()
    {
        var mountLines = TryReadMountLines();
        var healthByParentDisk = new Dictionary<string, string?>();

        return DriveInfo.GetDrives()
            .Where(drive => drive.IsReady)
            .Select(drive => (Drive: drive, Device: mountLines is null ? null : FindDeviceForMountPoint(mountLines, drive.Name)))
            .Where(x => mountLines is null || IsBlockDevice(x.Device))
            .Select(x => new DiskDto
            {
                Name = x.Drive.Name,
                Size = x.Drive.TotalSize,
                Used = x.Drive.TotalSize - x.Drive.TotalFreeSpace,
                IsOsDisk = x.Drive.Name == "/",
                HealthStatus = x.Device is null ? null : TryGetHealthStatus(x.Device, healthByParentDisk)
            })
            .ToList();
    }

    internal static bool IsBlockDevice(string? device)
    {
        return device is not null
               && device.StartsWith("/dev/", StringComparison.Ordinal)
               && !device.StartsWith("/dev/loop", StringComparison.Ordinal);
    }

    private static List<string>? TryReadMountLines()
    {
        try
        {
            return File.ReadLines("/proc/mounts").ToList();
        }
        catch
        {
            return null;
        }
    }

    private string? TryGetHealthStatus(string device, Dictionary<string, string?> healthByParentDisk)
    {
        try
        {
            return GetHealthForDevice(device, healthByParentDisk);
        }
        catch
        {
            return null;
        }
    }

    internal string? GetHealthForDevice(string device)
    {
        return GetHealthForDevice(device, new Dictionary<string, string?>());
    }

    internal string? GetHealthForDevice(string device, Dictionary<string, string?> healthByParentDisk)
    {
        var parentDisk = GetParentDisk(device);
        if (healthByParentDisk.TryGetValue(parentDisk, out var cachedHealth))
        {
            return cachedHealth;
        }

        var health = ParseSmartctlHealth(RunProcess("smartctl", $"-a -j {parentDisk}"));
        healthByParentDisk[parentDisk] = health;
        return health;
    }

    internal static string? ParseSmartctlHealth(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.TryGetProperty("smart_status", out var smartStatus) &&
            smartStatus.TryGetProperty("passed", out var passed))
        {
            return passed.GetBoolean() ? DiskHealthStatus.Ok : DiskHealthStatus.Failing;
        }

        return null;
    }

    internal static string? FindDeviceForMountPoint(IEnumerable<string> mountLines, string mountPoint)
    {
        foreach (var line in mountLines)
        {
            var parts = line.Split(' ');
            if (parts.Length >= 2 && parts[1].Replace(@"\040", " ") == mountPoint)
            {
                return parts[0];
            }
        }

        return null;
    }

    internal string GetParentDisk(string device)
    {
        var parentName = RunProcess("lsblk", $"-no pkname {device}").Trim();
        return string.IsNullOrEmpty(parentName) ? device : $"/dev/{parentName}";
    }

    private string RunProcess(string fileName, string arguments)
    {
        using var handle = processRunner.Start(fileName, arguments);
        var output = handle.ReadToEnd();
        handle.WaitForExit();
        return output;
    }
}
