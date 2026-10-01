using Mycelium.Api.Devices.Get.SoftwareInformation.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Get.SoftwareInformation.v1;

public class GetSoftwareInformationHandlerTests
{
    [Test]
    public async Task Handle_DeviceNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new GetSoftwareInformationHandler(dbContext);

        var result = await handler.Handle(new GetSoftwareInformationQuery(1), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_DeviceFound_ReturnsSoftwareOrderedByName()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var device = new Device { Name = "MyPc", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        device.Software.Add(new DeviceSoftware { Name = "Zed" });
        device.Software.Add(new DeviceSoftware { Name = "Alpha" });
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var handler = new GetSoftwareInformationHandler(dbContext);
        var result = await handler.Handle(new GetSoftwareInformationQuery(device.Id), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Select(s => s.Name), Is.EqualTo(["Alpha", "Zed"]));
    }
}
