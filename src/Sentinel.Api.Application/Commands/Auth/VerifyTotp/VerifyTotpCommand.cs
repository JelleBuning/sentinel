using Mediator;

namespace Sentinel.Api.Application.Commands.Auth.VerifyTotp;

public record VerifyTotpCommand(int UserId, string AuthenticityToken, string OtpAttempt) : IRequest<DTO.Token.TokenDto>;
