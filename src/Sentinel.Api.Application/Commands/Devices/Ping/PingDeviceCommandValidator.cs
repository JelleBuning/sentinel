using FluentValidation;

namespace Sentinel.Api.Application.Commands.Devices.Ping;

public class PingDeviceCommandValidator : AbstractValidator<PingDeviceCommand>
{
    public PingDeviceCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .GreaterThan(0).WithMessage("DeviceId must be greater than 0");
    }
}
