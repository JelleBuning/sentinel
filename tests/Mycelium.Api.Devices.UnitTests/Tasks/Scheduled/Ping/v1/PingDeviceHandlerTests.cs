using Mycelium.Api.Devices.Tasks.Scheduled.Ping.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Tasks.Scheduled.Ping.v1;

public class PingDeviceHandlerTests
{
    [Test]
    public async Task Handle_AccessGuardFails_ReturnsForbidden()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new PingDeviceHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(2));

        var result = await handler.Handle(new PingDeviceCommand(1), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new PingDeviceHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(1));

        var result = await handler.Handle(new PingDeviceCommand(1), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_DeviceFound_UpdatesLastActive()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now.AddDays(-1) };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var before = DateTime.Now;
        var handler = new PingDeviceHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));

        var result = await handler.Handle(new PingDeviceCommand(device.Id), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(device.LastActive, Is.GreaterThanOrEqualTo(before));
    }
}
