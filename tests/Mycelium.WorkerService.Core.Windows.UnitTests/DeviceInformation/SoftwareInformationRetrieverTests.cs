using Microsoft.Win32;
using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using Mycelium.WorkerService.Core.Windows.Registry;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

#pragma warning disable CA1416
public class SoftwareInformationRetrieverTests
{
    [Test]
    public void ShouldInclude_NullName_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude(null, false, false), Is.False);
    }

    [Test]
    public void ShouldInclude_WhitespaceName_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("   ", false, false), Is.False);
    }

    [Test]
    public void ShouldInclude_SystemComponent_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("Some App", true, false), Is.False);
    }

    [Test]
    public void ShouldInclude_Update_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("Some App", false, true), Is.False);
    }

    [Test]
    public void ShouldInclude_ValidNameNotSystemComponentNotUpdate_ReturnsTrue()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("Some App", false, false), Is.True);
    }

    [Test]
    public void Retrieve_FiltersOutSystemComponentsUpdatesAndBlankNames_KeepsValidEntries()
    {
        var registryService = Substitute.For<IRegistryService>();
        registryService.GetUninstallEntries(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            .Returns(new List<RegistryUninstallEntry>
            {
                new("Notepad++", false, false),
                new("Windows Update Component", true, false),
                new("KB123456 Update", false, true),
                new(null, false, false),
                new("   ", false, false)
            });
        registryService.GetUninstallEntries(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
            .Returns([]);
        registryService.GetUninstallEntries(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            .Returns([]);

        var retriever = new SoftwareInformationRetriever(registryService);

        var result = retriever.Retrieve();

        Assert.That(result.Select(x => x.Name), Is.EqualTo(new[] { "Notepad++" }));
    }

    [Test]
    public void Retrieve_QueriesAllThreeUninstallHiveAndPathCombinations()
    {
        var registryService = Substitute.For<IRegistryService>();
        registryService.GetUninstallEntries(Arg.Any<RegistryHive>(), Arg.Any<string>()).Returns([]);

        var retriever = new SoftwareInformationRetriever(registryService);

        retriever.Retrieve();

        registryService.Received(1).GetUninstallEntries(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
        registryService.Received(1).GetUninstallEntries(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
        registryService.Received(1).GetUninstallEntries(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
    }

    [Test]
    public void Retrieve_DuplicateNamesAcrossHives_DistinctResult()
    {
        var registryService = Substitute.For<IRegistryService>();
        registryService.GetUninstallEntries(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            .Returns(new List<RegistryUninstallEntry> { new("Shared App", false, false) });
        registryService.GetUninstallEntries(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
            .Returns(new List<RegistryUninstallEntry> { new("Shared App", false, false) });
        registryService.GetUninstallEntries(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            .Returns([]);

        var retriever = new SoftwareInformationRetriever(registryService);

        var result = retriever.Retrieve();

        Assert.That(result.Count, Is.EqualTo(1));
    }
}
