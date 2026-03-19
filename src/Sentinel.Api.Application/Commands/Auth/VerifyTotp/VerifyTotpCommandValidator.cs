using FluentValidation;

namespace Sentinel.Api.Application.Commands.Auth.VerifyTotp;

public class VerifyTotpCommandValidator : AbstractValidator<VerifyTotpCommand>
{
    public VerifyTotpCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0");

        RuleFor(x => x.AuthenticityToken)
            .NotEmpty().WithMessage("AuthenticityToken is required");

        RuleFor(x => x.OtpAttempt)
            .NotEmpty().WithMessage("OtpAttempt is required")
            .Length(6).WithMessage("OtpAttempt must be 6 digits");
    }
}
