using FluentValidation;

namespace Mycelium.Api.Devices.Tasks.Scheduled.Ping.v1;

public sealed class PingDeviceValidator : AbstractValidator<PingDeviceCommand>
{
    public PingDeviceValidator()
    {
        RuleFor(x => x.DeviceId)
            .GreaterThan(0).WithMessage("DeviceId must be greater than 0");
    }
}
