using NUnit.Framework;
using Mycelium.Api.Auth.Login.v1;

namespace Mycelium.Api.Auth.UnitTests.Login.v1;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Test]
    public void Validate_ValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.Validate(new LoginCommand("user@example.com", "password"));

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase("", "password")]
    [TestCase("not-an-email", "password")]
    public void Validate_InvalidEmail_ShouldHaveError(string email, string password)
    {
        var result = _validator.Validate(new LoginCommand(email, password));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Email"), Is.True);
    }

    [Test]
    public void Validate_EmptyPassword_ShouldHaveError()
    {
        var result = _validator.Validate(new LoginCommand("user@example.com", ""));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Password"), Is.True);
    }
}
