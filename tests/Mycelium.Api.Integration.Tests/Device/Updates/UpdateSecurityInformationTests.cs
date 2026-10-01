using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Common.DTO.Device;
using NUnit.Framework;

namespace Mycelium.Api.IntegrationTests.Device.Updates;

public class UpdateSecurityInformationTests
{
    [Test]
    public async Task AuthorizedDevice_UpdateSecurityInformation_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();
        
        var updateDto = new SecurityDto
        {
            LastSecurityScanDto = new LastSecurityScanDto
            {
                LastScan = DateTime.UtcNow.AddDays(-1),
                Duration = TimeSpan.FromMinutes(30)
            },
            AntivirusEnabled = true,
            RealTimeProtectionEnabled = true,
            FirewallSettingsDto = new FirewallSettingsDto
            {
                DomainFirewallEnabled = true,
                PrivateFirewallEnabled = true,
                PublicFirewallEnabled = true
            }
        };

        var device = scope.Organisation.Devices.Single();
        
        var result = await scope.Client.PutAsync($"/api/v1/devices/{device!.Id}/security", updateDto);
        result.ShouldBeOk();
    }

    [Test]
    public async Task UnauthorizedDevice_UpdateSecurityInformation_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        
        var updateDto = new SecurityDto
        {
            LastSecurityScanDto = new LastSecurityScanDto
            {
                LastScan = DateTime.UtcNow.AddDays(-1),
                Duration = TimeSpan.FromMinutes(30)
            },
            AntivirusEnabled = true,
            RealTimeProtectionEnabled = true,
            FirewallSettingsDto = new FirewallSettingsDto
            {
                DomainFirewallEnabled = true,
                PrivateFirewallEnabled = true,
                PublicFirewallEnabled = true
            }
        };

        var result = await scope.Client.PutAsync("/api/v1/devices/1/security", updateDto);
        
        result.ShouldBeUnauthorized();
    }
}
