using Mediator;
using Mycelium.Api.Application.DTO.Token;
using Mycelium.Api.Application.Interfaces;

namespace Mycelium.Api.Application.Commands.Auth.RefreshToken;

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
