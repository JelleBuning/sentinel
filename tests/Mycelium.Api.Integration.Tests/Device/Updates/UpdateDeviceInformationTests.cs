using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Common.DTO.Device.Information;
using NUnit.Framework;

namespace Mycelium.Api.IntegrationTests.Device.Updates;

public class UpdateDeviceInformationTests
{
    [Test]
    public async Task AuthorizedDevice_UpdateDeviceInformation_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();

        var updateDto = new InformationDto
        {
            DeviceName = "Updated Device",
            OsName = "Windows",
            OsVersion = "11",
            Version = "22H2",
            ProductName = "Windows 11 Pro",
            Processor = "Intel Core i7",
            InstalledRam = "16GB",
            GraphicsCard = "NVIDIA RTX 3080",
            Manufacturer = "Dell"
        };

        var device = scope.Organisation.Devices.Single();

        var result = await scope.Client.PutAsync($"/api/v1/devices/{device.Id}", updateDto);

        result.ShouldBeOk();
    }

    [Test]
    public async Task UnauthorizedDevice_UpdateDeviceInformation_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();

        var updateDto = new InformationDto
        {
            DeviceName = "Updated Device",
            OsName = "Windows",
            OsVersion = "11",
            Version = "22H2",
            ProductName = "Windows 11 Pro",
            Processor = "Intel Core i7",
            InstalledRam = "16GB",
            GraphicsCard = "NVIDIA RTX 3080",
            Manufacturer = "Dell"
        };

        var result = await scope.Client.PutAsync("/api/v1/devices/1", updateDto);

        result.ShouldBeUnauthorized();
    }
}
