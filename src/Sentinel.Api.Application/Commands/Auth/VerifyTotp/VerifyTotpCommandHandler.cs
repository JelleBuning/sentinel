using Mediator;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Auth.VerifyTotp;

public class VerifyTotpCommandHandler(IAuthRepository authRepository) : IRequestHandler<VerifyTotpCommand, TokenDto>
{
    public async ValueTask<TokenDto> Handle(VerifyTotpCommand request, CancellationToken cancellationToken)
    {
        var verifyDto = new VerifyUserDto
        {
            UserId = request.UserId,
            AuthenticityToken = request.AuthenticityToken,
            OtpAttempt = request.OtpAttempt
        };

        var user = await authRepository.VerifyTotpAsync(verifyDto);
        return await authRepository.GetTokenAsync(user);
    }
}
