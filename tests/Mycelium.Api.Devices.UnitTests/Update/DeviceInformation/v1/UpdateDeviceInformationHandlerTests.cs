using Mycelium.Api.Devices.Update.DeviceInformation.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Common.DTO.Device.Information;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Update.DeviceInformation.v1;

public class UpdateDeviceInformationHandlerTests
{
    [Test]
    public async Task Handle_AccessGuardFails_ReturnsForbidden()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateDeviceInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(2));

        var result = await handler.Handle(new UpdateDeviceInformationCommand(1, new InformationDto()), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new UpdateDeviceInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(1));

        var result = await handler.Handle(new UpdateDeviceInformationCommand(1, new InformationDto()), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_DeviceNameProvided_UpdatesDeviceNameAndInformation()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "OldName", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateDeviceInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var info = new InformationDto { DeviceName = "NewName", OsName = "Windows", OsVersion = "11" };

        var result = await handler.Handle(new UpdateDeviceInformationCommand(device.Id, info), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(device.Name, Is.EqualTo("NewName"));
        Assert.That(device.DeviceInformation.OsName, Is.EqualTo("Windows"));
        Assert.That(device.DeviceInformation.OsVersion, Is.EqualTo("11"));
    }

    [Test]
    public async Task Handle_DeviceNameNull_KeepsExistingDeviceName()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "OldName", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new UpdateDeviceInformationHandler(dbContext, TestHttpContextAccessorFactory.ForDevice(device.Id));
        var info = new InformationDto { DeviceName = null };

        var result = await handler.Handle(new UpdateDeviceInformationCommand(device.Id, info), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(device.Name, Is.EqualTo("OldName"));
    }
}
