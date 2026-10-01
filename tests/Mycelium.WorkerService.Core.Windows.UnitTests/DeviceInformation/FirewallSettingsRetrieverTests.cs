using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using Mycelium.WorkerService.Core.Windows.Wmi;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

public class FirewallSettingsRetrieverTests
{
    [Test]
    public void MapToFirewallSettings_AllProfilesEnabled_AllTrue()
    {
        var profiles = new List<(string? Name, string? Enabled)>
        {
            ("Domain", "1"),
            ("Private", "1"),
            ("Public", "1")
        };

        var result = FirewallSettingsRetriever.MapToFirewallSettings(profiles);

        Assert.That(result.DomainFirewallEnabled, Is.True);
        Assert.That(result.PrivateFirewallEnabled, Is.True);
        Assert.That(result.PublicFirewallEnabled, Is.True);
    }

    [Test]
    public void MapToFirewallSettings_ProfilesDisabledOrNotOne_AllFalse()
    {
        var profiles = new List<(string? Name, string? Enabled)>
        {
            ("Domain", "0"),
            ("Private", null),
            ("Public", "not-a-flag")
        };

        var result = FirewallSettingsRetriever.MapToFirewallSettings(profiles);

        Assert.That(result.DomainFirewallEnabled, Is.False);
        Assert.That(result.PrivateFirewallEnabled, Is.False);
        Assert.That(result.PublicFirewallEnabled, Is.False);
    }

    [Test]
    public void MapToFirewallSettings_MixedProfileStates_MapsIndependently()
    {
        var profiles = new List<(string? Name, string? Enabled)>
        {
            ("Domain", "1"),
            ("Private", "0"),
            ("Public", "1")
        };

        var result = FirewallSettingsRetriever.MapToFirewallSettings(profiles);

        Assert.That(result.DomainFirewallEnabled, Is.True);
        Assert.That(result.PrivateFirewallEnabled, Is.False);
        Assert.That(result.PublicFirewallEnabled, Is.True);
    }

    [Test]
    public void Retrieve_QueriesWmiAndMapsProfiles()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM MSFT_NetFirewallProfile", @"\\.\root\StandardCimv2")
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["Name"] = "Domain", ["Enabled"] = "1" },
                new Dictionary<string, object?> { ["Name"] = "Private", ["Enabled"] = "0" },
                new Dictionary<string, object?> { ["Name"] = "Public", ["Enabled"] = "1" }
            });

        var retriever = new FirewallSettingsRetriever(wmiQueryService);

        var result = retriever.Retrieve();

        Assert.That(result.DomainFirewallEnabled, Is.True);
        Assert.That(result.PrivateFirewallEnabled, Is.False);
        Assert.That(result.PublicFirewallEnabled, Is.True);
    }

    [Test]
    public void Retrieve_MissingEnabledProperty_TreatedAsNotEnabled()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query(Arg.Any<string>(), Arg.Any<string?>())
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["Name"] = "Domain" },
                new Dictionary<string, object?> { ["Name"] = "Private" },
                new Dictionary<string, object?> { ["Name"] = "Public" }
            });

        var retriever = new FirewallSettingsRetriever(wmiQueryService);

        var result = retriever.Retrieve();

        Assert.That(result.DomainFirewallEnabled, Is.False);
        Assert.That(result.PrivateFirewallEnabled, Is.False);
        Assert.That(result.PublicFirewallEnabled, Is.False);
    }
}
