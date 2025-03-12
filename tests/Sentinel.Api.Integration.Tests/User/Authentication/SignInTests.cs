using System.Net;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.WorkerService.Common.Api.Extensions;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class SignInTests
{
    [Test]
    public async Task Correct_Login_ShouldReturnOK()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        _ = await client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var result = await client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "test@test.com", Password = "password" });

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

    [Test]
    public async Task InvalidEmail_Login_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        _ = await client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var result = await client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "ahjfdkenfine@test.com", Password = "password" });

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}