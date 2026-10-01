using NUnit.Framework;
using Mycelium.Api.Auth.RefreshToken.v1;

namespace Mycelium.Api.Auth.UnitTests.RefreshToken.v1;

public class RefreshTokenValidatorTests
{
    private readonly RefreshTokenValidator _validator = new();

    [Test]
    public void Validate_ValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.Validate(new RefreshTokenCommand("access", "refresh"));

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_EmptyAccessToken_ShouldHaveError()
    {
        var result = _validator.Validate(new RefreshTokenCommand("", "refresh"));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "AccessToken"), Is.True);
    }

    [Test]
    public void Validate_EmptyRefreshToken_ShouldHaveError()
    {
        var result = _validator.Validate(new RefreshTokenCommand("access", ""));

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "RefreshToken"), Is.True);
    }
}
