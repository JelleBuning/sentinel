using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Domain.Entities;

namespace Sentinel.Api.Application.Interfaces;

public interface IAuthRepository
{
    Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
    Task<SignInUserResponse> AuthenticateAsync(SignInUserDto user);
    Task<TokenDto> GetTokenAsync(User user);
    Task<User> VerifyTotpAsync(VerifyUserDto verifyUserDto);
}