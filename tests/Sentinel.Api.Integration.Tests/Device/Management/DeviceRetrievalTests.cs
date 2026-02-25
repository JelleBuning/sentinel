using NUnit.Framework;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.Device.Management;

public class DeviceRetrievalTests
{
    [Test]
    public async Task Unauthorized_GetDevices_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        var result = await scope.Client.GetAsync("/devices");
        result.ShouldBeUnauthorized();
    }

    [Test]
    public async Task Authorized_GetDevices_ShouldReturnOK()
    {
        await using var scope = await new TestScope().AuthenticateAsUserAsync();
        
        var result = await scope.Client.GetAsync("/devices");

        result.ShouldBeOk();
    }

    [Test]
    public async Task Authorized_GetDevices_ShouldOnlyReturnOrganisationDevices()
    {
        await using var scope = await new TestScope().AuthenticateAsUserAsync();
        var organisation = scope.DbContext.Organisations.Single(x => x.Id == scope.User!.OrganisationId);
        
        var authorizedDevice = new Domain.Entities.Device
        {
            OrganisationId = organisation.Id,
            Name = "AuthorizedDevice",
        };
        var unauthorizedDevice = new Domain.Entities.Device
        {
            OrganisationId = organisation.Id + 1, 
            Name = "UnauthorizedOrganisationDevice"
        };
        await scope.Fixture.AddDeviceAsync(authorizedDevice);
        await scope.Fixture.AddDeviceAsync(unauthorizedDevice);

        var result = await scope.Client.GetAsync("/devices");
        var devices = await result.ShouldDeserializeTo<GetDevicesResponse>();
        result.ShouldBeOk();
        Assert.That(devices.Devices.Count, Is.EqualTo(1));
        Assert.That(devices.Devices.Any(d => d.Name == authorizedDevice.Name), Is.True);
        Assert.That(devices.Devices.Any(d => d.Name == unauthorizedDevice.Name), Is.False);
    }
}