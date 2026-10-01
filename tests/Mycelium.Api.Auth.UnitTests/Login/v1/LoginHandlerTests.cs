using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using Mycelium.Api.Auth.Login.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Auth.UnitTests.Login.v1;

public class LoginHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task Handle_UserNotFound_ShouldReturnUnauthorized()
    {
        await using var dbContext = CreateDbContext();
        var tokenGenerator = Substitute.For<ITokenGenerator>();
        var handler = new LoginHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new LoginCommand("missing@example.com", "password"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Unauthorized));
    }

    [Test]
    public async Task Handle_WrongPassword_ShouldReturnUnauthorized()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Users.Add(new User
        {
            OrganisationId = 1,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("correct-password")
        });
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        var handler = new LoginHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new LoginCommand("user@example.com", "wrong-password"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Unauthorized));
    }

    [Test]
    public async Task Handle_ValidCredentials_ShouldReturnSuccessAndGenerateAccessToken()
    {
        await using var dbContext = CreateDbContext();
        var user = new User
        {
            OrganisationId = 5,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            TwoFactorToken = "SOME2FASECRET"
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>()).Returns("access-token");

        var handler = new LoginHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new LoginCommand("USER@example.com", "correct-password"), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.UserId, Is.EqualTo(user.Id));
        Assert.That(result.Value.OrganisationId, Is.EqualTo(5));
        Assert.That(result.Value.AuthenticityToken, Is.EqualTo("access-token"));
        Assert.That(result.Value.TwoFactorToken, Is.EqualTo("SOME2FASECRET"));
    }

    [Test]
    public async Task Handle_ValidCredentialsAlreadyVerified_ShouldNotExposeTwoFactorToken()
    {
        await using var dbContext = CreateDbContext();
        var user = new User
        {
            OrganisationId = 5,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            TwoFactorToken = "SOME2FASECRET",
            LastVerified = DateTime.UtcNow
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>()).Returns("access-token");

        var handler = new LoginHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new LoginCommand("user@example.com", "correct-password"), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.TwoFactorToken, Is.Null);
    }
}
