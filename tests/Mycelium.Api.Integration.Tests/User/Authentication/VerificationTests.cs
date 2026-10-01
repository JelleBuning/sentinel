using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Auth.VerifyTotp.v1;
using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Api.Users.Register.v1;
using NUnit.Framework;
using OtpNet;

namespace Mycelium.Api.IntegrationTests.User.Authentication;

public class VerificationTests
{
    [Test]
    public async Task Correct_Login_ShouldReturnOK()
    {
        await using var scope = new TestScope();

        _ = await scope.Client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));
        var signInResponse = await scope.Client.PostAsync("/api/v1/auth/users/sign_in", new { Email = "test@test.com", Password = "password" });
        var signInResult = await signInResponse.Content.DeserializeAsync<SignInResponse>() ?? throw new Exception("verification response was null");

        var totp = new Totp(Base32Encoding.ToBytes(signInResult.TwoFactorToken), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var result = await scope.Client.PostAsync("/api/v1/auth/users/verify", new VerifyTotpCommand(
            signInResult.UserId,
            signInResult.AuthenticityToken,
            totp.ComputeTotp()));

        result.ShouldBeOk();
    }
}
