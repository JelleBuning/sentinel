using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Api.Users.Register.v1;
using NUnit.Framework;

namespace Mycelium.Api.IntegrationTests.User.Authentication;

public class SignInTests
{
    [Test]
    public async Task Correct_Login_ShouldReturnOK()
    {
        await using var scope = new TestScope();

        _ = await scope.Client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));
        var result = await scope.Client.PostAsync("/api/v1/auth/users/sign_in", new { Email = "test@test.com", Password = "password" });

        result.ShouldBeOk();
    }

    [Test]
    public async Task InvalidPassword_Login_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();

        _ = await scope.Client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));
        var result = await scope.Client.PostAsync("/api/v1/auth/users/sign_in", new { Email = "test@test.com", Password = "hl;asdfljasdjfdaflha;sihjefkldj;aslfjkdsa;dfjasd" });

        result.ShouldBeUnauthorized();
    }

    [Test]
    public async Task InvalidEmail_Login_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();

        _ = await scope.Client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));
        var result = await scope.Client.PostAsync("/api/v1/auth/users/sign_in", new { Email = "ahjfdkenfine@test.com", Password = "password" });

        result.ShouldBeUnauthorized();
    }
}
