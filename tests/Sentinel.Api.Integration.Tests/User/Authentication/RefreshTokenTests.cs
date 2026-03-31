using NUnit.Framework;
using OtpNet;
using Sentinel.Api.Application.Commands.Auth.RefreshToken;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class RefreshTokenTests
{
    [Test]
    public async Task ValidRefreshToken_ShouldReturnNewTokens()
    {
        await using var scope = new TestScope();
        
        var registerDto = new RegisterUserDto { Email = "test@test.com", Password = "password" };
        await scope.Client.PostAsync("/users/register", registerDto);
        
        var signInDto = new SignInUserDto { Email = "test@test.com", Password = "password" };
        var signInResponse = await scope.Client.PostAsync("/auth/users/sign_in", signInDto);
        var signInResult = await signInResponse.Content.DeserializeAsync<SignInUserResponse>();
        
        var totp = new Totp(Base32Encoding.ToBytes(signInResult!.TwoFactorToken), step: 30,
            mode: OtpHashMode.Sha1, totpSize: 6);
        
        var verifyDto = new VerifyUserDto
        {
            UserId = signInResult.UserId,
            AuthenticityToken = signInResult.AuthenticityToken,
            OtpAttempt = totp.ComputeTotp()
        };
        var verifyResponse = await scope.Client.PostAsync("/auth/users/verify", verifyDto);
        
        verifyResponse.ShouldBeOk();
        
        var tokens = await verifyResponse.Content.DeserializeAsync<TokenDto>();
        Assert.That(tokens, Is.Not.Null, "Tokens should not be null");
        Assert.That(tokens!.AccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(tokens.RefreshToken, Is.Not.Null.And.Not.Empty);
        
        var refreshCommand = new RefreshTokenCommand(
            tokens.AccessToken,
            tokens.RefreshToken
        );
        
        var result = await scope.Client.PostAsync("/auth/refresh", refreshCommand);
        
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
        
        var result = await scope.Client.PostAsync("/auth/refresh", refreshCommand);
        
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
        
        var result = await scope.Client.PostAsync("/auth/refresh", refreshCommand);
        
        result.ShouldBeBadRequest();
    }
}
