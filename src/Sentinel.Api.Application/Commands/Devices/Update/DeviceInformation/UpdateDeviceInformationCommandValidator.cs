using FluentValidation;

namespace Sentinel.Api.Application.Commands.Devices.Update.DeviceInformation;

public class UpdateDeviceInformationCommandValidator : AbstractValidator<UpdateDeviceInformationCommand>
{
    public UpdateDeviceInformationCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .GreaterThan(0).WithMessage("DeviceId must be greater than 0");

        RuleFor(x => x.DeviceInfo)
            .NotNull().WithMessage("DeviceInfo is required");
    }
}
