using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Common.DTO.Device;
using NUnit.Framework;

namespace Mycelium.Api.IntegrationTests.Device.Updates;

public class UpdateDisksTests
{
    [Test]
    public async Task AuthorizedDevice_UpdateDisks_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();

        var updateDto = new List<DiskDto>
        {
            new()
            {
                Name = "C:",
                IsOsDisk = true,
                Used = 250.5,
                Size = 500.0,
                HealthStatus = "OK"
            }
        };
        var device = scope.DbContext.Devices.Single();

        var result = await scope.Client.PutAsync($"/api/v1/devices/{device.Id}/disks", updateDto);

        result.ShouldBeOk();
    }

    [Test]
    public async Task AuthorizedDevice_UpdateDisksWithoutHealthInformation_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();

        var updateDto = new List<DiskDto>
        {
            new()
            {
                Name = "C:",
                IsOsDisk = true,
                Used = 250.5,
                Size = 500.0
            }
        };
        var device = scope.DbContext.Devices.Single();

        var result = await scope.Client.PutAsync($"/api/v1/devices/{device.Id}/disks", updateDto);

        result.ShouldBeOk();
    }

    [Test]
    public async Task UnauthorizedDevice_UpdateDisks_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();

        var updateDto = new List<DiskDto>
        {
            new()
            {
                Name = "C:",
                IsOsDisk = true,
                Used = 250.5,
                Size = 500.0
            }
        };

        var result = await scope.Client.PutAsync("/api/v1/devices/1/disks", updateDto);

        result.ShouldBeUnauthorized();
    }
}
