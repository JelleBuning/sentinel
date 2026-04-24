using NUnit.Framework;
using Mycelium.Api.Integration.Tests.Common;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Integration.Tests.Device.Updates;

public class UpdateStorageInformationTests
{
    [Test]
    public async Task AuthorizedDevice_UpdateStorageInformation_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();
        
        var updateDto = new StorageInformationDto
        {
            Disks =
            [
                new()
                {
                    Name = "C:",
                    IsOsDisk = true,
                    Used = 250.5,
                    Size = 500.0
                }
            ]
        };
        var device = scope.DbContext.Devices.Single();
        
        var result = await scope.Client.PutAsync($"/devices/{device.Id}/storage", updateDto);
        
        result.ShouldBeOk();
    }

    [Test]
    public async Task UnauthorizedDevice_UpdateStorageInformation_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        
        var updateDto = new StorageInformationDto
        {
            Disks =
            [
                new()
                {
                    Name = "C:",
                    IsOsDisk = true,
                    Used = 250.5,
                    Size = 500.0
                }
            ]
        };
        
        var result = await scope.Client.PutAsync("/devices/1/storage", updateDto);
        
        result.ShouldBeUnauthorized();
    }
}
