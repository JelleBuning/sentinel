using System.Net;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.WorkerService.Common.Api.Extensions;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class RegisterTests
{
    [Test]
    public async Task Correct_Registration_ShouldReturnOK()
    {
        // Arrange
        using var client = new ApiFixture().CreateClient();

        // Act
        var result = await client.PostAsync("/users/register", new RegisterUserDto
        {
            Email = "test@test.com",
            Password = "password",
        });

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

        var result1 = await client.PostAsync("/users/register", user);
        var result2 = await client.PostAsync("/users/register", user);

        // Assert
        Assert.That(result1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result2.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}