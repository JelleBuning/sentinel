using Mycelium.Api.Devices.Tasks.Scheduled.Ping.v1;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Tasks.Scheduled.Ping.v1;

public class PingDeviceValidatorTests
{
    private readonly PingDeviceValidator _validator = new();

    [Test]
    public void Validate_DeviceIdZero_IsInvalid()
    {
        var result = _validator.Validate(new PingDeviceCommand(0));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_DeviceIdNegative_IsInvalid()
    {
        var result = _validator.Validate(new PingDeviceCommand(-1));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_DeviceIdPositive_IsValid()
    {
        var result = _validator.Validate(new PingDeviceCommand(1));

        Assert.That(result.IsValid, Is.True);
    }
}
