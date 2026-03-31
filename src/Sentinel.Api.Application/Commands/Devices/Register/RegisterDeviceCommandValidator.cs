using FluentValidation;

namespace Sentinel.Api.Application.Commands.Devices.Register;

public class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(x => x.OrganisationHash)
            .NotEmpty().WithMessage("OrganisationHash is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
    }
}
