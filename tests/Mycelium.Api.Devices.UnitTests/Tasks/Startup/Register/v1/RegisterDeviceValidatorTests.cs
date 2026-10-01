using Mycelium.Api.Devices.Tasks.Startup.Register.v1;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Tasks.Startup.Register.v1;

public class RegisterDeviceValidatorTests
{
    private readonly RegisterDeviceValidator _validator = new();

    [Test]
    public void Validate_EmptyOrganisationHash_IsInvalid()
    {
        var result = _validator.Validate(new RegisterDeviceCommand(Guid.Empty, "MyPc"));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_EmptyName_IsInvalid()
    {
        var result = _validator.Validate(new RegisterDeviceCommand(Guid.NewGuid(), string.Empty));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_NameExceedsMaxLength_IsInvalid()
    {
        var result = _validator.Validate(new RegisterDeviceCommand(Guid.NewGuid(), new string('a', 101)));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidCommand_IsValid()
    {
        var result = _validator.Validate(new RegisterDeviceCommand(Guid.NewGuid(), "MyPc"));

        Assert.That(result.IsValid, Is.True);
    }
}
