using FluentValidation;

namespace Mycelium.Api.Devices.Tasks.Startup.Register.v1;

public sealed class RegisterDeviceValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceValidator()
    {
        RuleFor(x => x.OrganisationHash)
            .NotEmpty().WithMessage("OrganisationHash is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
    }
}
