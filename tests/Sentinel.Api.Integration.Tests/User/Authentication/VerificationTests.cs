using System.Net;
using System.Text;
using Newtonsoft.Json;
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
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        var registerContent = new StringContent(JsonConvert.SerializeObject(new RegisterUserDto { Email = "test@test.com", Password = "password" }), Encoding.UTF8, "application/json");
        _ = await client.PostAsync("/users/auth/register", registerContent);

        var signInContent = new StringContent(JsonConvert.SerializeObject(new SignInUserDto { Email = "test@test.com", Password = "password" }), Encoding.UTF8, "application/json");
        var signInResponse = await client.PostAsync("/users/auth/sign_in", signInContent);
        var signInUserResponse = JsonConvert.DeserializeObject<SignInUserResponse>(await signInResponse.Content.ReadAsStringAsync()) ?? throw new Exception("verification response was null");

        var totp = new Totp(Base32Encoding.ToBytes(signInUserResponse.TwoFactorToken), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var verifyContent = new StringContent(JsonConvert.SerializeObject(new VerifyUserDto
        {
            UserId = signInUserResponse.UserId,
            AuthenticityToken = signInUserResponse.AuthenticityToken,
            OtpAttempt = totp.ComputeTotp(),
        }), Encoding.UTF8, "application/json");
        var result = await client.PostAsync("/users/auth/verify", verifyContent);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task InvalidPassword_Login_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        var registerContent = new StringContent(JsonConvert.SerializeObject(new RegisterUserDto { Email = "test@test.com", Password = "password" }), Encoding.UTF8, "application/json");
        _ = await client.PostAsync("/users/auth/register", registerContent);

        var signInContent = new StringContent(JsonConvert.SerializeObject(new SignInUserDto { Email = "test@test.com", Password = "hl;asdfljasdjfdaflha;sihjefkldj;aslfjkdsa;dfjasd" }), Encoding.UTF8, "application/json");
        var result = await client.PostAsync("/users/auth/sign_in", signInContent);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}