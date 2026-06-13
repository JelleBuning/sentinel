using Mediator;
using Mycelium.Api.Application.DTO.Token;

namespace Mycelium.Api.Application.Commands.Auth.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<TokenDto>;
