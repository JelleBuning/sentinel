using Mycelium.Api.Devices.Get.Disks.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Get.Disks.v1;

public class GetDisksHandlerTests
{
    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new GetDisksHandler(dbContext);

        var result = await handler.Handle(new GetDisksQuery(1), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_DeviceFound_MapsDisks()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        device.Disks.Add(new DeviceDisk { Name = "C:", IsOsDisk = true, Used = 100, Size = 500, HealthStatus = "OK" });
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new GetDisksHandler(dbContext);
        var result = await handler.Handle(new GetDisksQuery(device.Id), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        var disk = result.Value.Single();
        Assert.That(disk.Name, Is.EqualTo("C:"));
        Assert.That(disk.IsOsDisk, Is.True);
        Assert.That(disk.Used, Is.EqualTo(100));
        Assert.That(disk.Size, Is.EqualTo(500));
        Assert.That(disk.HealthStatus, Is.EqualTo("OK"));
    }
}
