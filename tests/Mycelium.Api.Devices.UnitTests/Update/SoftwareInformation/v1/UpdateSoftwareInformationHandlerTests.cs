using Mycelium.Api.Devices.Update.SoftwareInformation.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Common.DTO.Device;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Update.SoftwareInformation.v1;

public class UpdateSoftwareInformationHandlerTests
{
    [Test]
    public async Task Handle_AccessGuardFails_ReturnsForbidden()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateSoftwareInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(2));

        var result = await handler.Handle(new UpdateSoftwareInformationCommand(1, []), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateSoftwareInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(1));

        var result = await handler.Handle(new UpdateSoftwareInformationCommand(1, []), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_NewSoftwareName_AddsSoftware()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateSoftwareInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var software = new List<SoftwareDto> { new() { Name = "Chrome" } };

        var result = await handler.Handle(new UpdateSoftwareInformationCommand(device.Id, software), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(dbContext.DeviceSoftware.Single().Name, Is.EqualTo("Chrome"));
    }

    [Test]
    public async Task Handle_ExistingSoftwareName_DoesNotDuplicate()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        device.Software.Add(new DeviceSoftware { Name = "Chrome" });
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateSoftwareInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var software = new List<SoftwareDto> { new() { Name = "Chrome" } };

        var result = await handler.Handle(new UpdateSoftwareInformationCommand(device.Id, software), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(dbContext.DeviceSoftware.Count(), Is.EqualTo(1));
    }
}
