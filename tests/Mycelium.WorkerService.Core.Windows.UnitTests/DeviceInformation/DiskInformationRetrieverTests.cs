using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using Mycelium.WorkerService.Core.Windows.Wmi;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

public class DiskInformationRetrieverTests
{
    [Test]
    public void MapToDiskDto_TrimsTrailingBackslashFromDriveName()
    {
        var health = new Dictionary<string, string?>();

        var result = DiskInformationRetriever.MapToDiskDto(@"C:\", 500, 250, true, health);

        Assert.That(result.Name, Is.EqualTo("C:"));
    }

    [Test]
    public void MapToDiskDto_HealthStatusPresentForTrimmedName_IsAssigned()
    {
        var health = new Dictionary<string, string?> { ["C:"] = "OK" };

        var result = DiskInformationRetriever.MapToDiskDto(@"C:\", 500, 250, true, health);

        Assert.That(result.HealthStatus, Is.EqualTo("OK"));
    }

    [Test]
    public void MapToDiskDto_DriveNotInHealthDictionary_HealthStatusIsNull()
    {
        var health = new Dictionary<string, string?> { ["D:"] = "OK" };

        var result = DiskInformationRetriever.MapToDiskDto(@"C:\", 500, 250, true, health);

        Assert.That(result.HealthStatus, Is.Null);
    }

    [Test]
    public void MapToDiskDto_PassesThroughSizeUsedAndIsOsDisk()
    {
        var health = new Dictionary<string, string?>();

        var result = DiskInformationRetriever.MapToDiskDto(@"D:\", 1000, 400, false, health);

        Assert.That(result.Size, Is.EqualTo(1000));
        Assert.That(result.Used, Is.EqualTo(400));
        Assert.That(result.IsOsDisk, Is.False);
    }

    [Test]
    public void GetDriveLettersForDisk_ReturnsDriveLettersFromAssociatorChain()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("Win32_DiskDriveToDiskPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "Disk #0, Partition #0" }
            });
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("Win32_LogicalDiskToPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "C:" }
            });

        var retriever = new DiskInformationRetriever(wmiQueryService);

        var driveLetters = retriever.GetDriveLettersForDisk(@"\\.\PHYSICALDRIVE0").ToList();

        Assert.That(driveLetters, Is.EqualTo(new[] { "C:" }));
    }

    [Test]
    public void GetHealthByDriveLetter_MapsStatusToEachDriveLetterOfDisk()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM Win32_DiskDrive")
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = @"\\.\PHYSICALDRIVE0", ["Status"] = "OK" }
            });
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("Win32_DiskDriveToDiskPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "Disk #0, Partition #0" }
            });
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("Win32_LogicalDiskToPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "C:" }
            });

        var retriever = new DiskInformationRetriever(wmiQueryService);

        var health = retriever.GetHealthByDriveLetter();

        Assert.That(health["C:"], Is.EqualTo("OK"));
    }

    [Test]
    public void GetHealthByDriveLetter_OneDiskThrows_OtherDisksStillReturned()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM Win32_DiskDrive")
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = @"\\.\PHYSICALDRIVE0", ["Status"] = "OK" },
                new Dictionary<string, object?> { ["DeviceID"] = @"\\.\PHYSICALDRIVE1", ["Status"] = "OK" }
            });

        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("PHYSICALDRIVE0") && q.Contains("Win32_DiskDriveToDiskPartition")))
            .Returns(_ => throw new InvalidOperationException("simulated WMI failure for disk 0"));
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("PHYSICALDRIVE1") && q.Contains("Win32_DiskDriveToDiskPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "Disk #1, Partition #0" }
            });
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("Win32_LogicalDiskToPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "D:" }
            });

        var retriever = new DiskInformationRetriever(wmiQueryService);

        var health = retriever.GetHealthByDriveLetter();

        Assert.That(health.ContainsKey("C:"), Is.False);
        Assert.That(health["D:"], Is.EqualTo("OK"));
    }

    [Test]
    public void GetHealthByDriveLetter_DiskWithNullDeviceId_IsSkipped()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM Win32_DiskDrive")
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["Status"] = "OK" }
            });

        var retriever = new DiskInformationRetriever(wmiQueryService);

        var health = retriever.GetHealthByDriveLetter();

        Assert.That(health, Is.Empty);
    }

    [TestCase("OK", "OK")]
    [TestCase("Degraded", "Degraded")]
    [TestCase("Stressed", "Degraded")]
    [TestCase("Pred Fail", "Failing")]
    [TestCase("Error", "Failing")]
    [TestCase("NonRecover", "Failing")]
    [TestCase("No Contact", "Unknown")]
    [TestCase(null, null)]
    public void NormalizeStatus_MapsWmiStatusToSharedHealthValues(string? wmiStatus, string? expected)
    {
        var result = DiskInformationRetriever.NormalizeStatus(wmiStatus);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetHealthByDriveLetter_PredFailStatus_IsReportedAsFailing()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM Win32_DiskDrive")
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = @"\\.\PHYSICALDRIVE0", ["Status"] = "Pred Fail" }
            });
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("Win32_DiskDriveToDiskPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "Disk #0, Partition #0" }
            });
        wmiQueryService.Query(Arg.Is<string>(q => q.Contains("Win32_LogicalDiskToPartition")))
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["DeviceID"] = "C:" }
            });

        var retriever = new DiskInformationRetriever(wmiQueryService);

        var health = retriever.GetHealthByDriveLetter();

        Assert.That(health["C:"], Is.EqualTo("Failing"));
    }
}
