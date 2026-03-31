using Mediator;
using Sentinel.Api.Application.DTO.Token;

namespace Sentinel.Api.Application.Commands.Auth.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<TokenDto>;
