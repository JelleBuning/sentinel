using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Api.Users.Register.v1;

namespace Mycelium.Api.Users.UnitTests.Register.v1;

public class RegisterUserHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task Handle_EmailAlreadyInUse_ShouldReturnForbidden()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Organisations.Add(new Organisation
        {
            Hash = Guid.NewGuid(),
            Users = new List<User>
            {
                new() { Email = "existing@example.com", Password = "hash" }
            }
        });
        await dbContext.SaveChangesAsync();

        var handler = new RegisterUserHandler(dbContext);

        var result = await handler.Handle(new RegisterUserCommand("EXISTING@example.com", "password123"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(Mycelium.Api.Core.Results.ErrorType.Forbidden));
    }

    [Test]
    public async Task Handle_NewEmail_ShouldCreateOrganisationAndUser()
    {
        await using var dbContext = CreateDbContext();
        var handler = new RegisterUserHandler(dbContext);

        var result = await handler.Handle(new RegisterUserCommand("new@example.com", "password123"), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);

        var organisation = await dbContext.Organisations.Include(o => o.Users).SingleAsync();
        var user = organisation.Users.Single();

        Assert.That(user.Email, Is.EqualTo("new@example.com"));
        Assert.That(BCrypt.Net.BCrypt.Verify("password123", user.Password), Is.True);
        Assert.That(user.TwoFactorToken, Is.Not.Null.And.Not.Empty);
        Assert.That(organisation.Hash, Is.Not.EqualTo(Guid.Empty));
    }
}
