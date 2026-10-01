using Mycelium.Api.Devices.Update.Disks.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Common.DTO.Device;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Update.Disks.v1;

public class UpdateDisksHandlerTests
{
    [Test]
    public async Task Handle_AccessGuardFails_ReturnsForbidden()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateDisksHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(2));

        var result = await handler.Handle(new UpdateDisksCommand(1, []), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateDisksHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(1));

        var result = await handler.Handle(new UpdateDisksCommand(1, []), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_NewDiskName_AddsDisk()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateDisksHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var disks = new List<DiskDto> { new() { Name = "C:", IsOsDisk = true, Used = 10, Size = 100, HealthStatus = "OK" } };

        var result = await handler.Handle(new UpdateDisksCommand(device.Id, disks), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        var stored = dbContext.DeviceDisks.Single();
        Assert.That(stored.Name, Is.EqualTo("C:"));
        Assert.That(stored.HealthStatus, Is.EqualTo("OK"));
    }

    [Test]
    public async Task Handle_ExistingDiskName_UpdatesDiskInPlace()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        device.Disks.Add(new DeviceDisk { Name = "C:", IsOsDisk = true, Used = 10, Size = 100, HealthStatus = "OK" });
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateDisksHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var disks = new List<DiskDto> { new() { Name = "C:", IsOsDisk = true, Used = 50, Size = 100, HealthStatus = "Failing" } };

        var result = await handler.Handle(new UpdateDisksCommand(device.Id, disks), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        var stored = dbContext.DeviceDisks.Single();
        Assert.That(stored.Used, Is.EqualTo(50));
        Assert.That(stored.HealthStatus, Is.EqualTo("Failing"));
    }

    [Test]
    public async Task Handle_DiskMissingFromPayload_RemovesDisk()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        device.Disks.Add(new DeviceDisk { Name = "C:", IsOsDisk = true, Used = 10, Size = 100, HealthStatus = "OK" });
        device.Disks.Add(new DeviceDisk { Name = "E:", IsOsDisk = false, Used = 10, Size = 100, HealthStatus = "Failing" });
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateDisksHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var disks = new List<DiskDto> { new() { Name = "C:", IsOsDisk = true, Used = 20, Size = 100, HealthStatus = "OK" } };

        var result = await handler.Handle(new UpdateDisksCommand(device.Id, disks), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        var stored = dbContext.DeviceDisks.Single();
        Assert.That(stored.Name, Is.EqualTo("C:"));
        Assert.That(stored.Used, Is.EqualTo(20));
    }
}
