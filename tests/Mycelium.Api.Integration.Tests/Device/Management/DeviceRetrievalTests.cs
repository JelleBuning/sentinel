using Mycelium.Api.Devices.Get.All.v1;
using Mycelium.Api.IntegrationTests.Common;
using NUnit.Framework;
using DeviceEntity = Mycelium.Api.EntityFramework.Entities.Device;

namespace Mycelium.Api.IntegrationTests.Device.Management;

public class DeviceRetrievalTests
{
    [Test]
    public async Task Unauthorized_GetDevices_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        var result = await scope.Client.GetAsync("/api/v1/devices");
        result.ShouldBeUnauthorized();
    }

    [Test]
    public async Task Authorized_GetDevices_ShouldReturnOK()
    {
        await using var scope = await new TestScope().AuthenticateAsUserAsync();

        var result = await scope.Client.GetAsync("/api/v1/devices");

        result.ShouldBeOk();
    }

    [Test]
    public async Task Authorized_GetDevices_ShouldOnlyReturnOrganisationDevices()
    {
        await using var scope = await new TestScope().AuthenticateAsUserAsync();
        var organisation = scope.DbContext.Organisations.Single(x => x.Id == scope.User!.OrganisationId);

        var authorizedDevice = new DeviceEntity
        {
            OrganisationId = organisation.Id,
            Name = "AuthorizedDevice",
        };
        var unauthorizedDevice = new DeviceEntity
        {
            OrganisationId = organisation.Id + 1,
            Name = "UnauthorizedOrganisationDevice"
        };
        await scope.Fixture.AddDeviceAsync(authorizedDevice);
        await scope.Fixture.AddDeviceAsync(unauthorizedDevice);

        var result = await scope.Client.GetAsync("/api/v1/devices");
        result.ShouldBeOk();
        var devices = await result.ShouldDeserializeTo<DevicesResponse>();
        Assert.That(devices.Devices.Count, Is.EqualTo(1));
        Assert.That(devices.Devices.Any(d => d.Name == authorizedDevice.Name), Is.True);
        Assert.That(devices.Devices.Any(d => d.Name == unauthorizedDevice.Name), Is.False);
    }
}
