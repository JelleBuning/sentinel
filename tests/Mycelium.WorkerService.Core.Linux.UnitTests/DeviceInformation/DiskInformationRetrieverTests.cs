using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.Core.Linux.DeviceInformation;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Linux.UnitTests.DeviceInformation;

public class DiskInformationRetrieverTests
{
    private static (DiskInformationRetriever Retriever, IProcessRunner ProcessRunner) CreateRetriever()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        return (new DiskInformationRetriever(processRunner), processRunner);
    }

    private static IProcessHandle CreateHandle(string output)
    {
        var handle = Substitute.For<IProcessHandle>();
        handle.ReadToEnd().Returns(output);
        return handle;
    }

    [Test]
    public void GetParentDisk_LsblkReturnsParentName_ReturnsDevPrefixedName()
    {
        var (retriever, processRunner) = CreateRetriever();
        var lsblkHandle = CreateHandle("sda\n");
        processRunner.Start("lsblk", "-no pkname /dev/sda1").Returns(lsblkHandle);

        var result = retriever.GetParentDisk("/dev/sda1");

        Assert.That(result, Is.EqualTo("/dev/sda"));
    }

    [Test]
    public void GetParentDisk_LsblkReturnsEmpty_ReturnsOriginalDevice()
    {
        var (retriever, processRunner) = CreateRetriever();
        var lsblkHandle = CreateHandle(string.Empty);
        processRunner.Start("lsblk", "-no pkname /dev/sda").Returns(lsblkHandle);

        var result = retriever.GetParentDisk("/dev/sda");

        Assert.That(result, Is.EqualTo("/dev/sda"));
    }

    [Test]
    public void GetHealthForDevice_SmartctlReportsPassed_ReturnsOk()
    {
        var (retriever, processRunner) = CreateRetriever();
        var lsblkHandle = CreateHandle("sda\n");
        var smartctlHandle = CreateHandle("""{"smart_status":{"passed":true}}""");
        processRunner.Start("lsblk", "-no pkname /dev/sda1").Returns(lsblkHandle);
        processRunner.Start("smartctl", "-a -j /dev/sda").Returns(smartctlHandle);

        var result = retriever.GetHealthForDevice("/dev/sda1");

        Assert.That(result, Is.EqualTo("OK"));
    }

    [Test]
    public void GetHealthForDevice_SmartctlReportsFailing_ReturnsFailing()
    {
        var (retriever, processRunner) = CreateRetriever();
        var lsblkHandle = CreateHandle("sda\n");
        var smartctlHandle = CreateHandle("""{"smart_status":{"passed":false}}""");
        processRunner.Start("lsblk", "-no pkname /dev/sda1").Returns(lsblkHandle);
        processRunner.Start("smartctl", "-a -j /dev/sda").Returns(smartctlHandle);

        var result = retriever.GetHealthForDevice("/dev/sda1");

        Assert.That(result, Is.EqualTo("Failing"));
    }

    [Test]
    public void GetHealthForDevice_SmartctlReturnsNoOutput_ReturnsNull()
    {
        var (retriever, processRunner) = CreateRetriever();
        var lsblkHandle = CreateHandle("sda\n");
        var smartctlHandle = CreateHandle(string.Empty);
        processRunner.Start("lsblk", "-no pkname /dev/sda1").Returns(lsblkHandle);
        processRunner.Start("smartctl", "-a -j /dev/sda").Returns(smartctlHandle);

        var result = retriever.GetHealthForDevice("/dev/sda1");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseSmartctlHealth_PassedTrue_ReturnsOk()
    {
        const string json = """{"smart_status":{"passed":true}}""";

        var result = DiskInformationRetriever.ParseSmartctlHealth(json);

        Assert.That(result, Is.EqualTo("OK"));
    }

    [Test]
    public void ParseSmartctlHealth_PassedFalse_ReturnsFailing()
    {
        const string json = """{"smart_status":{"passed":false}}""";

        var result = DiskInformationRetriever.ParseSmartctlHealth(json);

        Assert.That(result, Is.EqualTo("Failing"));
    }

    [Test]
    public void ParseSmartctlHealth_MissingSmartStatusProperty_ReturnsNull()
    {
        const string json = """{"other_property":123}""";

        var result = DiskInformationRetriever.ParseSmartctlHealth(json);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseSmartctlHealth_EmptyString_ReturnsNull()
    {
        var result = DiskInformationRetriever.ParseSmartctlHealth(string.Empty);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseSmartctlHealth_WhitespaceString_ReturnsNull()
    {
        var result = DiskInformationRetriever.ParseSmartctlHealth("   ");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseSmartctlHealth_MalformedJson_Throws()
    {
        Assert.That(() => DiskInformationRetriever.ParseSmartctlHealth("not-json"),
            Throws.InstanceOf<System.Text.Json.JsonException>());
    }

    [Test]
    public void FindDeviceForMountPoint_MatchFound_ReturnsDevice()
    {
        var lines = new[]
        {
            "/dev/sda1 / ext4 rw,relatime 0 0",
            "/dev/sda2 /boot ext4 rw,relatime 0 0"
        };

        var result = DiskInformationRetriever.FindDeviceForMountPoint(lines, "/boot");

        Assert.That(result, Is.EqualTo("/dev/sda2"));
    }

    [Test]
    public void FindDeviceForMountPoint_NoMatch_ReturnsNull()
    {
        var lines = new[] { "/dev/sda1 / ext4 rw,relatime 0 0" };

        var result = DiskInformationRetriever.FindDeviceForMountPoint(lines, "/data");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void FindDeviceForMountPoint_LineWithFewerThanTwoFields_IsSkipped()
    {
        var lines = new[] { "malformed-line", "/dev/sda1 / ext4 rw,relatime 0 0" };

        var result = DiskInformationRetriever.FindDeviceForMountPoint(lines, "/");

        Assert.That(result, Is.EqualTo("/dev/sda1"));
    }

    [TestCase("/dev/sda1", true)]
    [TestCase("/dev/nvme0n1p2", true)]
    [TestCase("/dev/mapper/vg-root", true)]
    [TestCase("/dev/loop3", false)]
    [TestCase("proc", false)]
    [TestCase("tmpfs", false)]
    [TestCase("cgroup2", false)]
    [TestCase(null, false)]
    public void IsBlockDevice_OnlyAcceptsRealBlockDevices(string? device, bool expected)
    {
        Assert.That(DiskInformationRetriever.IsBlockDevice(device), Is.EqualTo(expected));
    }

    [Test]
    public void FindDeviceForMountPoint_MountPointWithEscapedSpace_ReturnsDevice()
    {
        var lines = new[] { @"/dev/sdb1 /mnt/my\040disk ext4 rw,relatime 0 0" };

        var result = DiskInformationRetriever.FindDeviceForMountPoint(lines, "/mnt/my disk");

        Assert.That(result, Is.EqualTo("/dev/sdb1"));
    }

    [Test]
    public void GetHealthForDevice_PartitionsOnSameDisk_RunSmartctlOnce()
    {
        var (retriever, processRunner) = CreateRetriever();
        processRunner.Start("lsblk", "-no pkname /dev/sda1").Returns(_ => CreateHandle("sda\n"));
        processRunner.Start("lsblk", "-no pkname /dev/sda2").Returns(_ => CreateHandle("sda\n"));
        processRunner.Start("smartctl", "-a -j /dev/sda").Returns(_ => CreateHandle("""{"smart_status":{"passed":true}}"""));
        var cache = new Dictionary<string, string?>();

        var first = retriever.GetHealthForDevice("/dev/sda1", cache);
        var second = retriever.GetHealthForDevice("/dev/sda2", cache);

        Assert.That(first, Is.EqualTo("OK"));
        Assert.That(second, Is.EqualTo("OK"));
        processRunner.Received(1).Start("smartctl", "-a -j /dev/sda");
    }
}
