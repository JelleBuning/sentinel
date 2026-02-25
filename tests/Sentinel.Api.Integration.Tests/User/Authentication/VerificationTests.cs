using NUnit.Framework;
using OtpNet;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class VerificationTests
{
    [Test]
    public async Task Correct_Login_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        
        _ = await scope.Client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var signInResponse = await scope.Client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "test@test.com", Password = "password" });
        var signInUserResponse = await signInResponse.Content.DeserializeAsync<SignInUserResponse>() ?? throw new Exception("verification response was null") ;

        var totp = new Totp(Base32Encoding.ToBytes(signInUserResponse.TwoFactorToken), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var result = await scope.Client.PostAsync("/auth/users/verify", new VerifyUserDto
        {
            UserId = signInUserResponse.UserId,
            AuthenticityToken = signInUserResponse.AuthenticityToken,
            OtpAttempt = totp.ComputeTotp(),
        });

        result.ShouldBeOk();
    }

    [Test]
    public async Task InvalidPassword_Login_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        
        _ = await scope.Client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var result = await scope.Client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "test@test.com", Password = "hl;asdfljasdjfdaflha;sihjefkldj;aslfjkdsa;dfjasd" });

        result.ShouldBeUnauthorized();
    }
}