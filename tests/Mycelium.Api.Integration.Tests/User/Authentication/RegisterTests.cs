using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Api.Users.Register.v1;
using NUnit.Framework;

namespace Mycelium.Api.IntegrationTests.User.Authentication;

public class RegisterTests
{
    [Test]
    public async Task Correct_Registration_ShouldReturnOK()
    {
        await using var scope = new TestScope();

        var result = await scope.Client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));

        result.ShouldBeOk();
    }

    [Test]
    public async Task DuplicateEmail_Registration_ShouldReturnForbidden()
    {
        await using var scope = new TestScope();

        var user = new RegisterUserCommand("test@test.com", "password");

        var result1 = await scope.Client.PostAsync("/api/v1/users/register", user);
        var result2 = await scope.Client.PostAsync("/api/v1/users/register", user);

        result1.ShouldBeOk();
        result2.ShouldBeForbidden();
    }
}
