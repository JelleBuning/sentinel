using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using Mycelium.Api.Auth.RefreshToken.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Auth.UnitTests.RefreshToken.v1;

public class RefreshTokenHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static ClaimsPrincipal CreatePrincipal(string id, string role)
    {
        var identity = new ClaimsIdentity([new Claim("Id", id), new Claim(ClaimTypes.Role, role)]);
        return new ClaimsPrincipal(identity);
    }

    [Test]
    public async Task Handle_UserRoleValidRefreshToken_ShouldReturnSuccessAndRotateToken()
    {
        await using var dbContext = CreateDbContext();
        var user = new User
        {
            OrganisationId = 1,
            Email = "user@example.com",
            Password = "hash",
            RefreshToken = "valid-refresh-token",
            RefreshTokenExpiryTime = DateTime.Now.AddDays(1)
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GetPrincipalFromExpiredToken("expired-access-token").Returns(CreatePrincipal(user.Id.ToString(), "User"));
        tokenGenerator.GenerateRefreshToken().Returns("new-refresh-token");
        tokenGenerator.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>()).Returns("new-access-token");

        var handler = new RefreshTokenHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new RefreshTokenCommand("expired-access-token", "valid-refresh-token"), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.AccessToken, Is.EqualTo("new-access-token"));
        Assert.That(result.Value.RefreshToken, Is.EqualTo("new-refresh-token"));
        Assert.That(user.RefreshToken, Is.EqualTo("new-refresh-token"));
    }

    [Test]
    public async Task Handle_UserRoleMismatchedRefreshToken_ShouldReturnValidationError()
    {
        await using var dbContext = CreateDbContext();
        var user = new User
        {
            OrganisationId = 1,
            Email = "user@example.com",
            Password = "hash",
            RefreshToken = "valid-refresh-token",
            RefreshTokenExpiryTime = DateTime.Now.AddDays(1)
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GetPrincipalFromExpiredToken(Arg.Any<string>()).Returns(CreatePrincipal(user.Id.ToString(), "User"));

        var handler = new RefreshTokenHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new RefreshTokenCommand("expired-access-token", "wrong-refresh-token"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Validation));
    }

    [Test]
    public async Task Handle_UserRoleExpiredRefreshToken_ShouldReturnValidationError()
    {
        await using var dbContext = CreateDbContext();
        var user = new User
        {
            OrganisationId = 1,
            Email = "user@example.com",
            Password = "hash",
            RefreshToken = "valid-refresh-token",
            RefreshTokenExpiryTime = DateTime.Now.AddDays(-1)
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GetPrincipalFromExpiredToken(Arg.Any<string>()).Returns(CreatePrincipal(user.Id.ToString(), "User"));

        var handler = new RefreshTokenHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new RefreshTokenCommand("expired-access-token", "valid-refresh-token"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Validation));
    }

    [Test]
    public async Task Handle_DeviceRoleValidRefreshToken_ShouldReturnSuccessAndRotateToken()
    {
        await using var dbContext = CreateDbContext();
        var device = new Device
        {
            OrganisationId = 1,
            Name = "TestDevice",
            RefreshToken = "valid-refresh-token"
        };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GetPrincipalFromExpiredToken(Arg.Any<string>()).Returns(CreatePrincipal(device.Id.ToString(), "Device"));
        tokenGenerator.GenerateRefreshToken().Returns("new-refresh-token");
        tokenGenerator.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>()).Returns("new-access-token");

        var handler = new RefreshTokenHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new RefreshTokenCommand("expired-access-token", "valid-refresh-token"), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(device.RefreshToken, Is.EqualTo("new-refresh-token"));
    }

    [Test]
    public async Task Handle_DeviceRoleMismatchedRefreshToken_ShouldReturnValidationError()
    {
        await using var dbContext = CreateDbContext();
        var device = new Device
        {
            OrganisationId = 1,
            Name = "TestDevice",
            RefreshToken = "valid-refresh-token"
        };
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GetPrincipalFromExpiredToken(Arg.Any<string>()).Returns(CreatePrincipal(device.Id.ToString(), "Device"));

        var handler = new RefreshTokenHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new RefreshTokenCommand("expired-access-token", "wrong-refresh-token"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Validation));
    }

    [Test]
    public void Handle_UnknownRole_ShouldThrowInvalidOperationException()
    {
        var dbContext = CreateDbContext();
        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GetPrincipalFromExpiredToken(Arg.Any<string>()).Returns(CreatePrincipal("1", "Unknown"));

        var handler = new RefreshTokenHandler(dbContext, tokenGenerator);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await handler.Handle(new RefreshTokenCommand("expired-access-token", "token"), CancellationToken.None));
    }
}
