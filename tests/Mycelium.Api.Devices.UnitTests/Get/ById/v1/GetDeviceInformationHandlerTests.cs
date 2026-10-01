using Mycelium.Api.Devices.Get.ById.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Get.ById.v1;

public class GetDeviceInformationHandlerTests
{
    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new GetDeviceInformationHandler(dbContext);

        var result = await handler.Handle(new GetDeviceInformationQuery(1), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_DeviceFound_MapsDeviceInformation()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device
        {
            Name = "MyPc",
            CreatedOn = DateTime.Now,
            LastActive = DateTime.Now,
            DeviceInformation = new DeviceInformation
            {
                OsName = "Windows",
                OsVersion = "11",
                Version = "23H2",
                ProductName = "XPS",
                Processor = "i7",
                InstalledRam = "32GB",
                GraphicsCard = "RTX",
                Manufacturer = "Dell"
            }
        };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new GetDeviceInformationHandler(dbContext);
        var result = await handler.Handle(new GetDeviceInformationQuery(device.Id), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.DeviceName, Is.EqualTo("MyPc"));
        Assert.That(result.Value.OsName, Is.EqualTo("Windows"));
        Assert.That(result.Value.OsVersion, Is.EqualTo("11"));
        Assert.That(result.Value.Version, Is.EqualTo("23H2"));
        Assert.That(result.Value.ProductName, Is.EqualTo("XPS"));
        Assert.That(result.Value.Processor, Is.EqualTo("i7"));
        Assert.That(result.Value.InstalledRam, Is.EqualTo("32GB"));
        Assert.That(result.Value.GraphicsCard, Is.EqualTo("RTX"));
        Assert.That(result.Value.Manufacturer, Is.EqualTo("Dell"));
    }
}
