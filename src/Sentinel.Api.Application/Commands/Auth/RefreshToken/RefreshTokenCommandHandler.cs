using Mediator;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Auth.RefreshToken;

public class RefreshTokenCommandHandler(IAuthRepository authRepository) : IRequestHandler<RefreshTokenCommand, TokenDto>
{
    public async ValueTask<TokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenDto = new TokenDto
        {
            AccessToken = request.AccessToken,
            RefreshToken = request.RefreshToken
        };

        return await authRepository.RefreshTokenAsync(tokenDto);
    }
}
