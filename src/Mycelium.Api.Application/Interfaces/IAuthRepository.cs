using Mycelium.Api.Application.DTO.Token;
using Mycelium.Api.Application.DTO.User;
using Mycelium.Api.Domain.Entities;

namespace Mycelium.Api.Application.Interfaces;

public interface IAuthRepository
{
    Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
    Task<SignInUserResponse> AuthenticateAsync(SignInUserDto user);
    Task<TokenDto> GetTokenAsync(User user);
    Task<User> VerifyTotpAsync(VerifyUserDto verifyUserDto);
}