using NUnit.Framework;
using Mycelium.Api.Application.DTO.User;
using Mycelium.Api.Integration.Tests.Common;

namespace Mycelium.Api.Integration.Tests.User.Authentication;

public class RegisterTests
{
    [Test]
    public async Task Correct_Registration_ShouldReturnOK()
    {
        await using var scope = new TestScope();

        var result = await scope.Client.PostAsync("/users/register", new RegisterUserDto
        {
            Email = "test@test.com",
            Password = "password",
        });

        result.ShouldBeOk();
    }

    [Test]
    public async Task DuplicateEmail_Registration_ShouldReturnForbidden()
    {
        await using var scope = new TestScope();

        var user = new RegisterUserDto
        {
            Email = "test@test.com",
            Password = "password",
        };

        var result1 = await scope.Client.PostAsync("/users/register", user);
        var result2 = await scope.Client.PostAsync("/users/register", user);

        result1.ShouldBeOk();
        result2.ShouldBeForbidden();
    }
}