using Mycelium.Api.Devices.Update.DeviceInformation.v1;
using Mycelium.Common.DTO.Device.Information;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Update.DeviceInformation.v1;

public class UpdateDeviceInformationValidatorTests
{
    private readonly UpdateDeviceInformationValidator _validator = new();

    [Test]
    public void Validate_DeviceIdZero_IsInvalid()
    {
        var result = _validator.Validate(new UpdateDeviceInformationCommand(0, new InformationDto()));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_DeviceInfoNull_IsInvalid()
    {
        var result = _validator.Validate(new UpdateDeviceInformationCommand(1, null!));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidCommand_IsValid()
    {
        var result = _validator.Validate(new UpdateDeviceInformationCommand(1, new InformationDto()));

        Assert.That(result.IsValid, Is.True);
    }
}
