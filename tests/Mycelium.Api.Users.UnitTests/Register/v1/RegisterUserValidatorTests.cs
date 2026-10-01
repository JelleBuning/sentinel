using NUnit.Framework;
using Mycelium.Api.Users.Register.v1;

namespace Mycelium.Api.Users.UnitTests.Register.v1;

public class RegisterUserValidatorTests
{
    private readonly RegisterUserValidator _validator = new();

    [Test]
    public void Validate_ValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.Validate(new RegisterUserCommand("user@example.com", "password123"));

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase("")]
    [TestCase("not-an-email")]
    public void Validate_InvalidEmail_ShouldHaveError(string email)
    {
        var result = _validator.Validate(new RegisterUserCommand(email, "password123"));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Email"), Is.True);
    }

    [TestCase("")]
    [TestCase("short")]
    public void Validate_InvalidPassword_ShouldHaveError(string password)
    {
        var result = _validator.Validate(new RegisterUserCommand("user@example.com", password));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Password"), Is.True);
    }
}
