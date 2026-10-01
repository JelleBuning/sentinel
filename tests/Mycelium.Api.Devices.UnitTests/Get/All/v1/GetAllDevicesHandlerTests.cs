using Mycelium.Api.Devices.Get.All.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Get.All.v1;

public class GetAllDevicesHandlerTests
{
    [Test]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var handler = new GetAllDevicesHandler(dbContext);

        var result = await handler.Handle(new GetAllDevicesQuery(1), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_UserFound_ReturnsOnlyOrganisationDevicesWithActiveCount()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var organisation = new Organisation { Hash = Guid.NewGuid() };
        var otherOrganisation = new Organisation { Hash = Guid.NewGuid() };
        var user = new User { Email = "a@b.com", Password = "hash", Organisation = organisation, OrganisationId = 0 };
        organisation.Users.Add(user);

        var activeDevice = new Device { Name = "Active", CreatedOn = DateTime.Now, LastActive = DateTime.Now };
        var inactiveDevice = new Device { Name = "Inactive", CreatedOn = DateTime.Now, LastActive = DateTime.Now.AddMinutes(-10) };
        organisation.Devices.Add(activeDevice);
        organisation.Devices.Add(inactiveDevice);
        otherOrganisation.Devices.Add(new Device { Name = "OtherOrgDevice", CreatedOn = DateTime.Now, LastActive = DateTime.Now });

        dbContext.Organisations.AddRange(organisation, otherOrganisation);
        await dbContext.SaveChangesAsync();

        var handler = new GetAllDevicesHandler(dbContext);
        var result = await handler.Handle(new GetAllDevicesQuery(user.Id), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.TotalDevices, Is.EqualTo(2));
        Assert.That(result.Value.ActiveDevices, Is.EqualTo(1));
        Assert.That(result.Value.OrganisationHash, Is.EqualTo(organisation.Hash));
        Assert.That(result.Value.Devices.Select(d => d.Name), Is.EquivalentTo(["Active", "Inactive"]));
    }
}
