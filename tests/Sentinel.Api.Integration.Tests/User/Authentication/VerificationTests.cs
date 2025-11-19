using System.Net;
using NUnit.Framework;
using OtpNet;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.WorkerService.Common.Api.Extensions;
using Sentinel.WorkerService.Common.Extensions;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class VerificationTests
{
    [Test]
    public async Task Correct_Login_ShouldReturnOK()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();
        
        // Act
        _ = await client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var signInResponse = await client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "test@test.com", Password = "password" });
        var signInUserResponse = await signInResponse.Content.DeserializeAsync<SignInUserResponse>() ?? throw new Exception("verification response was null") ;

        var totp = new Totp(Base32Encoding.ToBytes(signInUserResponse.TwoFactorToken), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var result = await client.PostAsync("/auth/users/verify", new VerifyUserDto
        {
            UserId = signInUserResponse.UserId,
            AuthenticityToken = signInUserResponse.AuthenticityToken,
            OtpAttempt = totp.ComputeTotp(),
        });

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task InvalidPassword_Login_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();
        // Act
        _ = await client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var result = await client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "test@test.com", Password = "hl;asdfljasdjfdaflha;sihjefkldj;aslfjkdsa;dfjasd" });

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}