using NUnit.Framework;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Integration.Tests.Device.Updates;

public class UpdateSecurityInformationTests
{
    [Test]
    public async Task AuthorizedDevice_UpdateSecurityInformation_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();
        
        var updateDto = new SecurityInformationDto
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
        
        var result = await scope.Client.PutAsync($"/devices/{device!.Id}/security", updateDto);
        result.ShouldBeOk();
    }

    [Test]
    public async Task UnauthorizedDevice_UpdateSecurityInformation_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        
        var updateDto = new SecurityInformationDto
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
        
        var result = await scope.Client.PutAsync("/devices/1/security", updateDto);
        
        result.ShouldBeUnauthorized();
    }
}
