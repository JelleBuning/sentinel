using Sentinel.Api.Domain.Entities;
using System.Security.Claims;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;

namespace Sentinel.Api.Application.Interfaces;

public interface IUserRepository
{
    Task Register(RegisterUserDto user);
    public Task<SignInUserResponse> Authenticate(SignInUserDto user);
    TokenResponse GetToken(User user);
    Task<User?> GetByClaims(IEnumerable<Claim> claims);
    Task<User> VerifyTotp(VerifyUserDto verifyUserDto);
    TokenResponse RefreshToken(RefreshTokenDto refreshTokenDto);
}