using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Auth.RefreshToken.v1;
using Mycelium.Api.Auth.VerifyTotp.v1;
using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Api.Users.Register.v1;
using NUnit.Framework;
using OtpNet;

namespace Mycelium.Api.IntegrationTests.User.Authentication;

public class RefreshTokenTests
{
    [Test]
    public async Task ValidRefreshToken_ShouldReturnNewTokens()
    {
        await using var scope = new TestScope();

        await scope.Client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));

        var signInResponse = await scope.Client.PostAsync("/api/v1/auth/users/sign_in", new { Email = "test@test.com", Password = "password" });
        var signInResult = await signInResponse.Content.DeserializeAsync<SignInResponse>();

        var totp = new Totp(Base32Encoding.ToBytes(signInResult!.TwoFactorToken), step: 30,
            mode: OtpHashMode.Sha1, totpSize: 6);

        var verifyTotpCommand = new VerifyTotpCommand(
            signInResult.UserId,
            signInResult.AuthenticityToken,
            totp.ComputeTotp());
        var verifyResponse = await scope.Client.PostAsync("/api/v1/auth/users/verify", verifyTotpCommand);

        verifyResponse.ShouldBeOk();

        var tokens = await verifyResponse.Content.DeserializeAsync<TokenDto>();
        Assert.That(tokens, Is.Not.Null, "Tokens should not be null");
        Assert.That(tokens!.AccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(tokens.RefreshToken, Is.Not.Null.And.Not.Empty);

        var refreshCommand = new RefreshTokenCommand(
            tokens.AccessToken,
            tokens.RefreshToken
        );

        var result = await scope.Client.PostAsync("/api/v1/auth/refresh", refreshCommand);

        result.ShouldBeOk();
        var newTokens = await result.ShouldDeserializeTo<TokenDto>();
        Assert.That(newTokens.AccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(newTokens.RefreshToken, Is.Not.Null.And.Not.Empty);
        Assert.That(newTokens.AccessToken, Is.Not.EqualTo(tokens.AccessToken));
    }

    [Test]
    public async Task InvalidRefreshToken_ShouldReturnError()
    {
        await using var scope = new TestScope();

        var refreshCommand = new RefreshTokenCommand(
            "invalid-access-token",
            "invalid-refresh-token"
        );

        var result = await scope.Client.PostAsync("/api/v1/auth/refresh", refreshCommand);

        Assert.That(result.IsSuccessStatusCode, Is.False, "Invalid tokens should not return success");
    }

    [Test]
    public async Task EmptyTokens_ShouldReturnBadRequest()
    {
        await using var scope = new TestScope();

        var refreshCommand = new RefreshTokenCommand(
            string.Empty,
            string.Empty
        );

        var result = await scope.Client.PostAsync("/api/v1/auth/refresh", refreshCommand);

        result.ShouldBeBadRequest();
    }
}
