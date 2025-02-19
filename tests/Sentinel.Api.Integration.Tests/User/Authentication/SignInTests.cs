using System.Net;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class SignInTests
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
        var result = await client.PostAsync("/users/auth/sign_in", signInContent);

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
    
    [Test]
    public async Task InvalidEmail_Login_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        var registerContent = new StringContent(JsonConvert.SerializeObject(new RegisterUserDto { Email = "test@test.com", Password = "password" }), Encoding.UTF8, "application/json");
        _ = await client.PostAsync("/users/auth/register", registerContent);

        var signInContent = new StringContent(JsonConvert.SerializeObject(new SignInUserDto { Email = "ahjfdkenfine@test.com", Password = "password" }), Encoding.UTF8, "application/json");
        var result = await client.PostAsync("/users/auth/sign_in", signInContent);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}