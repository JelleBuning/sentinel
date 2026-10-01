using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Common.DTO.Device;
using NUnit.Framework;
using DeviceDiskEntity = Mycelium.Api.EntityFramework.Entities.DeviceDisk;

namespace Mycelium.Api.IntegrationTests.Device.Management;

public class DiskRetrievalTests
{
    [Test]
    public async Task Unauthorized_GetDisks_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        var result = await scope.Client.GetAsync("/api/v1/devices/1/disks");
        result.ShouldBeUnauthorized();
    }

    [Test]
    public async Task Authorized_GetDisks_ShouldReturnReportedHealthFields()
    {
        await using var scope = await new TestScope().AuthenticateAsUserAsync();
        await scope.AddDeviceAsync();
        var device = scope.Organisation.Devices.Single();

        device.Disks.Add(new DeviceDiskEntity
        {
            Name = "C:",
            IsOsDisk = true,
            Used = 250.5,
            Size = 500.0,
            HealthStatus = "OK"
        });
        await scope.DbContext.SaveChangesAsync();

        var result = await scope.Client.GetAsync($"/api/v1/devices/{device.Id}/disks");

        result.ShouldBeOk();
        var disks = await result.ShouldDeserializeTo<List<DiskDto>>();
        var disk = disks.Single();
        Assert.That(disk.HealthStatus, Is.EqualTo("OK"));
    }
}
