using NUnit.Framework;
using Mycelium.Api.Auth.VerifyTotp.v1;

namespace Mycelium.Api.Auth.UnitTests.VerifyTotp.v1;

public class VerifyTotpValidatorTests
{
    private readonly VerifyTotpValidator _validator = new();

    [Test]
    public void Validate_ValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.Validate(new VerifyTotpCommand(1, "token", "123456"));

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_UserIdZero_ShouldHaveError()
    {
        var result = _validator.Validate(new VerifyTotpCommand(0, "token", "123456"));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "UserId"), Is.True);
    }

    [Test]
    public void Validate_EmptyAuthenticityToken_ShouldHaveError()
    {
        var result = _validator.Validate(new VerifyTotpCommand(1, "", "123456"));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "AuthenticityToken"), Is.True);
    }

    [TestCase("")]
    [TestCase("12345")]
    [TestCase("1234567")]
    public void Validate_InvalidOtpAttemptLength_ShouldHaveError(string otpAttempt)
    {
        var result = _validator.Validate(new VerifyTotpCommand(1, "token", otpAttempt));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "OtpAttempt"), Is.True);
    }
}
