using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Api.Organisations.GetAll.v1;

namespace Mycelium.Api.Organisations.UnitTests.GetAll.v1;

public class GetAllOrganisationsHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task Handle_NoOrganisations_ShouldReturnEmptyList()
    {
        await using var dbContext = CreateDbContext();
        var handler = new GetAllOrganisationsHandler(dbContext);

        var result = await handler.Handle(new GetAllOrganisationsQuery(), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Empty);
    }

    [Test]
    public async Task Handle_OrganisationsWithUsersAndDevices_ShouldMapNestedCollections()
    {
        await using var dbContext = CreateDbContext();
        var lastActive = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var hash = Guid.NewGuid();

        dbContext.Organisations.Add(new Organisation
        {
            Hash = hash,
            Users = new List<User>
            {
                new() { Email = "user1@example.com", Password = "hash1" },
                new() { Email = "user2@example.com", Password = "hash2" }
            },
            Devices = new List<Device>
            {
                new() { Name = "Device1", LastActive = lastActive }
            }
        });
        await dbContext.SaveChangesAsync();

        var handler = new GetAllOrganisationsHandler(dbContext);

        var result = await handler.Handle(new GetAllOrganisationsQuery(), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        var organisation = result.Value.Single();
        Assert.That(organisation.Hash, Is.EqualTo(hash));
        Assert.That(organisation.Users, Has.Count.EqualTo(2));
        Assert.That(organisation.Users.Any(u => u.Email == "user1@example.com"), Is.True);
        Assert.That(organisation.Devices, Has.Count.EqualTo(1));
        Assert.That(organisation.Devices.Single().Name, Is.EqualTo("Device1"));
        Assert.That(organisation.Devices.Single().LastActive, Is.EqualTo(lastActive));
    }
}
