using Mycelium.Api.Devices.Update.SecurityInformation.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Common.DTO.Device;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Update.SecurityInformation.v1;

public class UpdateSecurityInformationHandlerTests
{
    private static SecurityDto CreateSecurityDto() => new()
    {
        LastSecurityScanDto = new LastSecurityScanDto { LastScan = DateTime.Now, Duration = TimeSpan.FromMinutes(3) },
        AntivirusEnabled = true,
        RealTimeProtectionEnabled = true,
        NisEnabled = true,
        TamperProtectionEnabled = true,
        AntispywareEnabled = true,
        IsVirtualMachine = false,
        FirewallSettingsDto = new FirewallSettingsDto
        {
            DomainFirewallEnabled = true,
            PrivateFirewallEnabled = true,
            PublicFirewallEnabled = false
        }
    };

    [Test]
    public async Task Handle_AccessGuardFails_ReturnsForbidden()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateSecurityInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(2));

        var result = await handler.Handle(new UpdateSecurityInformationCommand(1, CreateSecurityDto()), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateSecurityInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(1));

        var result = await handler.Handle(new UpdateSecurityInformationCommand(1, CreateSecurityDto()), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_DeviceFound_UpdatesSecurityAndFirewallFields()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateSecurityInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var dto = CreateSecurityDto();

        var result = await handler.Handle(new UpdateSecurityInformationCommand(device.Id, dto), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(device.DeviceSecurity.AntivirusEnabled, Is.True);
        Assert.That(device.DeviceSecurity.DomainFirewallEnabled, Is.True);
        Assert.That(device.DeviceSecurity.PrivateFirewallEnabled, Is.True);
        Assert.That(device.DeviceSecurity.PublicFirewallEnabled, Is.False);
        Assert.That(device.DeviceSecurity.LastScan, Is.EqualTo(dto.LastSecurityScanDto.LastScan));
    }
}
