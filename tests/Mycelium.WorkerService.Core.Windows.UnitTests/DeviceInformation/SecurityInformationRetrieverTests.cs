using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using Mycelium.WorkerService.Core.Windows.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.Windows.Wmi;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

public class SecurityInformationRetrieverTests
{
    [Test]
    public void ParseExact_ValidWmiDateTimeString_ReturnsParsedDateTime()
    {
        var result = SecurityInformationRetriever.ParseExact("20260115103045.123456+000");

        Assert.That(result, Is.EqualTo(new DateTime(2026, 1, 15, 10, 30, 45, 123).AddTicks(4560)));
    }

    [Test]
    public void ParseExact_MalformedInput_Throws()
    {
        Assert.Throws<FormatException>(() => SecurityInformationRetriever.ParseExact("not-a-date"));
    }

    [Test]
    public void Retrieve_MapsWmiRowAndFirewallSettingsIntoSecurityDto()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM MSFT_MpComputerStatus", @"\\.\root\Microsoft\Windows\Defender")
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?>
                {
                    ["AntivirusEnabled"] = true,
                    ["AntivirusSignatureLastUpdated"] = "20260115103045.123456+000",
                    ["AntispywareSignatureLastUpdated"] = "20260116103045.123456+000",
                    ["RealTimeProtectionEnabled"] = true,
                    ["NISEnabled"] = false,
                    ["IsTamperProtected"] = true,
                    ["AntispywareEnabled"] = false,
                    ["IsVirtualMachine"] = true,
                    ["QuickScanStartTime"] = "20260117103045.123456+000"
                }
            });

        var firewallSettingsRetriever = Substitute.For<IFirewallSettingsRetriever>();
        var firewallSettings = new FirewallSettingsDto
        {
            DomainFirewallEnabled = true,
            PrivateFirewallEnabled = false,
            PublicFirewallEnabled = true
        };
        firewallSettingsRetriever.Retrieve().Returns(firewallSettings);

        var retriever = new SecurityInformationRetriever(wmiQueryService, firewallSettingsRetriever);

        var result = retriever.Retrieve();

        Assert.That(result.AntivirusEnabled, Is.True);
        Assert.That(result.LastAntivirusUpdate, Is.EqualTo(new DateTime(2026, 1, 15, 10, 30, 45, 123).AddTicks(4560)));
        Assert.That(result.LastAntispywareUpdate, Is.EqualTo(new DateTime(2026, 1, 16, 10, 30, 45, 123).AddTicks(4560)));
        Assert.That(result.RealTimeProtectionEnabled, Is.True);
        Assert.That(result.NisEnabled, Is.False);
        Assert.That(result.TamperProtectionEnabled, Is.True);
        Assert.That(result.AntispywareEnabled, Is.False);
        Assert.That(result.IsVirtualMachine, Is.True);
        Assert.That(result.FirewallSettingsDto, Is.SameAs(firewallSettings));
    }

    [Test]
    public void Retrieve_BooleanPropertiesNullOrMissing_DefaultToFalse()
    {
        var wmiQueryService = Substitute.For<IWmiQueryService>();
        wmiQueryService.Query("SELECT * FROM MSFT_MpComputerStatus", @"\\.\root\Microsoft\Windows\Defender")
            .Returns(new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?>
                {
                    ["AntivirusEnabled"] = null,
                    ["AntivirusSignatureLastUpdated"] = "20260115103045.123456+000",
                    ["AntispywareSignatureLastUpdated"] = "20260116103045.123456+000",
                    ["RealTimeProtectionEnabled"] = null,
                    ["NISEnabled"] = null,
                    ["IsTamperProtected"] = null,
                    ["AntispywareEnabled"] = null,
                    ["IsVirtualMachine"] = null
                }
            });
        var firewallSettingsRetriever = Substitute.For<IFirewallSettingsRetriever>();
        firewallSettingsRetriever.Retrieve().Returns(new FirewallSettingsDto());

        var result = new SecurityInformationRetriever(wmiQueryService, firewallSettingsRetriever).Retrieve();

        Assert.That(result.AntivirusEnabled, Is.False);
        Assert.That(result.AntispywareEnabled, Is.False);
        Assert.That(result.RealTimeProtectionEnabled, Is.False);
    }
}
