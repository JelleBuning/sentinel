using NUnit.Framework;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.Common.DTO.Device;

namespace Sentinel.Api.Integration.Tests.Device.Updates;

public class UpdateSoftwareInformationTests
{
    [Test]
    public async Task AuthorizedDevice_UpdateSoftwareInformation_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();
        
        var updateDto = new SoftwareInformationDto
        {
            Software = new List<SoftwareDto>
            {
                new() { Name = "Google Chrome" },
                new() { Name = "Mozilla Firefox" },
                new() { Name = "Visual Studio Code" }
            }
        };
        var device = scope.Organisation.Devices.Single();
        
        var result = await scope.Client.PutAsync($"/devices/{device!.Id}/software", updateDto);
        
        result.ShouldBeOk();
    }

    [Test]
    public async Task UnauthorizedDevice_UpdateSoftwareInformation_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        
        var updateDto = new SoftwareInformationDto
        {
            Software = new List<SoftwareDto>
            {
                new() { Name = "Google Chrome" },
                new() { Name = "Mozilla Firefox" },
                new() { Name = "Visual Studio Code" }
            }
        };
        
        var result = await scope.Client.PutAsync("/devices/1/software", updateDto);
        
        result.ShouldBeUnauthorized();
    }
}
