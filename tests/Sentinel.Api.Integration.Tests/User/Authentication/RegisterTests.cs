using System.Net;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class RegisterTests
{
    [Test]
    public async Task Correct_Registration_ShouldReturnOK()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        var content = new StringContent(JsonSerializer.Serialize(new RegisterUserDto
        {
            Email = "test@test.com",
            Password = "password",
        }), Encoding.UTF8, "application/json");

        var result = await client.PostAsync("/users/auth/register", content);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task DuplicateEmail_Registration_ShouldReturnForbidden()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        var user = new RegisterUserDto
        {
            Email = "test@test.com",
            Password = "password",
        };
        var content1 = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

        var result1 = await client.PostAsync("/users/auth/register", content1);
        var result2 = await client.PostAsync("/users/auth/register", content2);

        // Assert
        Assert.That(result1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result2.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}