using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using OtpNet;
using Mycelium.Api.Auth.VerifyTotp.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Auth.UnitTests.VerifyTotp.v1;

public class VerifyTotpHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task Handle_UserNotFound_ShouldReturnValidationError()
    {
        await using var dbContext = CreateDbContext();
        var tokenGenerator = Substitute.For<ITokenGenerator>();
        var handler = new VerifyTotpHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new VerifyTotpCommand(1, "token", "123456"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Validation));
    }

    [Test]
    public async Task Handle_AuthenticityTokenMismatch_ShouldReturnUnauthorized()
    {
        await using var dbContext = CreateDbContext();
        var user = new User
        {
            OrganisationId = 1,
            Email = "user@example.com",
            Password = "hash",
            AuthenticityToken = "correct-token",
            TwoFactorToken = Base32Encoding.ToString("12345678901234567890"u8.ToArray())
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        var handler = new VerifyTotpHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new VerifyTotpCommand(user.Id, "wrong-token", "123456"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Unauthorized));
    }

    [Test]
    public async Task Handle_InvalidOtpAttempt_ShouldReturnUnauthorized()
    {
        await using var dbContext = CreateDbContext();
        var secret = Base32Encoding.ToString("12345678901234567890"u8.ToArray());
        var user = new User
        {
            OrganisationId = 1,
            Email = "user@example.com",
            Password = "hash",
            AuthenticityToken = "correct-token",
            TwoFactorToken = secret
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        var handler = new VerifyTotpHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new VerifyTotpCommand(user.Id, "correct-token", "000000"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Unauthorized));
    }

    [Test]
    public async Task Handle_ValidOtpAttempt_ShouldReturnSuccessAndUpdateUser()
    {
        await using var dbContext = CreateDbContext();
        var secret = Base32Encoding.ToString("12345678901234567890"u8.ToArray());
        var user = new User
        {
            OrganisationId = 1,
            Email = "user@example.com",
            Password = "hash",
            AuthenticityToken = "correct-token",
            TwoFactorToken = secret,
            Type = UserType.User
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var totp = new Totp(Base32Encoding.ToBytes(secret), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var code = totp.ComputeTotp();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GenerateRefreshToken().Returns("new-refresh-token");
        tokenGenerator.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>()).Returns("new-access-token");

        var handler = new VerifyTotpHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new VerifyTotpCommand(user.Id, "correct-token", code), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.AccessToken, Is.EqualTo("new-access-token"));
        Assert.That(result.Value.RefreshToken, Is.EqualTo("new-refresh-token"));
        Assert.That(user.LastVerified, Is.Not.Null);
        Assert.That(user.RefreshToken, Is.EqualTo("new-refresh-token"));
    }
}
