using Mycelium.Api.Devices.Get.SecurityInformation.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Get.SecurityInformation.v1;

public class GetSecurityInformationHandlerTests
{
    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new GetSecurityInformationHandler(dbContext);

        var result = await handler.Handle(new GetSecurityInformationQuery(1), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_DeviceFound_MapsSecurityAndFirewallInformation()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var lastScan = DateTime.Now.AddDays(-1);
        var device = new Device
        {
            Name = "MyPc",
            CreatedOn = DateTime.Now,
            LastActive = DateTime.Now,
            DeviceSecurity = new DeviceSecurity
            {
                LastScan = lastScan,
                Duration = TimeSpan.FromMinutes(5),
                AntivirusEnabled = true,
                RealTimeProtectionEnabled = true,
                NisEnabled = true,
                TamperProtectionEnabled = true,
                AntispywareEnabled = true,
                IsVirtualMachine = false,
                DomainFirewallEnabled = true,
                PrivateFirewallEnabled = false,
                PublicFirewallEnabled = true
            }
        };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new GetSecurityInformationHandler(dbContext);
        var result = await handler.Handle(new GetSecurityInformationQuery(device.Id), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.LastSecurityScanDto.LastScan, Is.EqualTo(lastScan));
        Assert.That(result.Value.LastSecurityScanDto.Duration, Is.EqualTo(TimeSpan.FromMinutes(5)));
        Assert.That(result.Value.AntivirusEnabled, Is.True);
        Assert.That(result.Value.FirewallSettingsDto.DomainFirewallEnabled, Is.True);
        Assert.That(result.Value.FirewallSettingsDto.PrivateFirewallEnabled, Is.False);
        Assert.That(result.Value.FirewallSettingsDto.PublicFirewallEnabled, Is.True);
    }
}
