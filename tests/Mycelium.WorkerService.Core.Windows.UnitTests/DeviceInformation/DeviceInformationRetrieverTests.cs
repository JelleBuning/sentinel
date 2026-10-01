using Mycelium.WorkerService.Common.Hardware;
using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using Mycelium.WorkerService.Core.Windows.Wmi;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

public class DeviceInformationRetrieverTests
{
    private static IWmiQueryService BuildWmiQueryService()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();

        wmiQueryService.Query("SELECT * FROM Win32_OperatingSystem", Arg.Any<string?>())
            .Returns(Row("Caption", "Windows 11 Pro"));
        wmiQueryService.Query("SELECT * FROM Win32_ComputerSystem", Arg.Any<string?>())
            .Returns(Row("Manufacturer", "Dell Inc."));
        wmiQueryService.Query("SELECT * FROM Win32_ComputerSystemProduct", Arg.Any<string?>())
            .Returns(Row("Name", "XPS 16"));
        wmiQueryService.Query("SELECT * FROM Win32_Processor", Arg.Any<string?>())
            .Returns(Row("Name", "Intel Core Ultra 9"));
        wmiQueryService.Query("SELECT * FROM Win32_VideoController", Arg.Any<string?>())
            .Returns(Row("Caption", "NVIDIA GeForce RTX"));

        return wmiQueryService;
    }

    private static List<IReadOnlyDictionary<string, object?>> Row(string key, object? value)
    {
        return [new Dictionary<string, object?> { [key] = value }];
    }

    [Test]
    public void Retrieve_MapsEachWmiClassToTheRightField()
    {
        var wmiQueryService = BuildWmiQueryService();
        var memoryInfoProvider = Substitute.For<IMemoryInfoProvider>();
        memoryInfoProvider.GetInstalledMemoryKilobytes().Returns(33_554_432L); // 32 GiB in KB

        var retriever = new DeviceInformationRetriever(wmiQueryService, memoryInfoProvider);

        var result = retriever.Retrieve();

        Assert.That(result.OsName, Is.EqualTo("Windows 11 Pro"));
        Assert.That(result.Manufacturer, Is.EqualTo("Dell Inc."));
        Assert.That(result.ProductName, Is.EqualTo("XPS 16"));
        Assert.That(result.Processor, Is.EqualTo("Intel Core Ultra 9"));
        Assert.That(result.GraphicsCard, Is.EqualTo("NVIDIA GeForce RTX"));
        Assert.That(result.DeviceName, Is.EqualTo(Environment.MachineName));
    }

    [Test]
    public void Retrieve_ConvertsInstalledMemoryFromKilobytesToMegabytesString()
    {
        var wmiQueryService = BuildWmiQueryService();
        var memoryInfoProvider = Substitute.For<IMemoryInfoProvider>();
        memoryInfoProvider.GetInstalledMemoryKilobytes().Returns(16_777_216L); // 16 GiB in KB

        var retriever = new DeviceInformationRetriever(wmiQueryService, memoryInfoProvider);

        var result = retriever.Retrieve();

        Assert.That(result.InstalledRam, Is.EqualTo((16_777_216L / 1024 / 1024).ToString()));
    }

    [Test]
    public void Retrieve_MultipleRowsForSameClass_JoinsValuesWithComma()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM Win32_VideoController", Arg.Any<string?>())
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["Caption"] = "GPU One" },
                new Dictionary<string, object?> { ["Caption"] = "GPU Two" }
            });
        wmiQueryService.Query("SELECT * FROM Win32_OperatingSystem", Arg.Any<string?>()).Returns([]);
        wmiQueryService.Query("SELECT * FROM Win32_ComputerSystem", Arg.Any<string?>()).Returns([]);
        wmiQueryService.Query("SELECT * FROM Win32_ComputerSystemProduct", Arg.Any<string?>()).Returns([]);
        wmiQueryService.Query("SELECT * FROM Win32_Processor", Arg.Any<string?>()).Returns([]);

        var memoryInfoProvider = Substitute.For<IMemoryInfoProvider>();
        memoryInfoProvider.GetInstalledMemoryKilobytes().Returns(0L);

        var retriever = new DeviceInformationRetriever(wmiQueryService, memoryInfoProvider);

        var result = retriever.Retrieve();

        Assert.That(result.GraphicsCard, Is.EqualTo("GPU One, GPU Two"));
    }

    [Test]
    public void Retrieve_MissingProperty_IgnoredAndReturnsEmptyString()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM Win32_OperatingSystem", Arg.Any<string?>())
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["SomeOtherProperty"] = "value" }
            });
        wmiQueryService.Query("SELECT * FROM Win32_ComputerSystem", Arg.Any<string?>()).Returns([]);
        wmiQueryService.Query("SELECT * FROM Win32_ComputerSystemProduct", Arg.Any<string?>()).Returns([]);
        wmiQueryService.Query("SELECT * FROM Win32_Processor", Arg.Any<string?>()).Returns([]);
        wmiQueryService.Query("SELECT * FROM Win32_VideoController", Arg.Any<string?>()).Returns([]);

        var memoryInfoProvider = Substitute.For<IMemoryInfoProvider>();
        memoryInfoProvider.GetInstalledMemoryKilobytes().Returns(0L);

        var retriever = new DeviceInformationRetriever(wmiQueryService, memoryInfoProvider);

        var result = retriever.Retrieve();

        Assert.That(result.OsName, Is.EqualTo(string.Empty));
    }
}
