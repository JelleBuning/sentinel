using NUnit.Framework;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.User.Authentication;

public class SignInTests
{
    [Test]
    public async Task Correct_Login_ShouldReturnOK()
    {
        await using var scope = new TestScope();

        _ = await scope.Client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var result = await scope.Client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "test@test.com", Password = "password" });

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

    [Test]
    public async Task InvalidEmail_Login_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();

        _ = await scope.Client.PostAsync("/users/register", new RegisterUserDto { Email = "test@test.com", Password = "password" });
        var result = await scope.Client.PostAsync("/auth/users/sign_in", new SignInUserDto { Email = "ahjfdkenfine@test.com", Password = "password" });

        result.ShouldBeUnauthorized();
    }
}