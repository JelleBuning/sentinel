using Microsoft.EntityFrameworkCore;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Domain.Entities;
using Sentinel.Api.Infrastructure.Persistence;
using System.Security.Claims;
using OtpNet;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.Services.Interfaces;
using Sentinel.Api.Infrastructure.Exceptions;

namespace Sentinel.Api.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext, ITokenService tokenService) : IUserRepository
{
    public async Task<User?> GetByClaims(IEnumerable<Claim> claims)
    {
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == email);
        return user;
    }

    public async Task Register(RegisterUserDto user)
    {
        var userExists = dbContext.Users.FirstOrDefault(x => x.Email.ToLower() == user.Email.ToLower());
        if (userExists != null)
        {
            throw new Exception("Email already in use", new ForbiddenException());
        }

        var key = KeyGeneration.GenerateRandomKey(20);
        var base32String = Base32Encoding.ToString(key);

        var organisation = new Organisation
        {
            Hash = Guid.NewGuid(),
            Users = new List<User>
            {
                new()
                {
                    Email = user.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    TwoFactorToken = base32String,
                }
            }
        };

        await dbContext.Organisations.AddAsync(organisation);
        await dbContext.SaveChangesAsync();
    }


    public async Task<SignInUserResponse> Authenticate(SignInUserDto user)
    {
        var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(user.Email.ToLower()));
        if (currentUser == null) throw new Exception("Invalid login credentials", new UnauthorizedException());
        
        var passwordValid = BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password);
        if (!passwordValid) throw new Exception("Invalid login credentials", new UnauthorizedException());
            
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
        };

        currentUser.AuthenticityToken = tokenService.GenerateAccessToken(claims);
        await dbContext.SaveChangesAsync();
        
        return new SignInUserResponse
        {
            UserId = currentUser.Id,
            OrganisationId = currentUser.OrganisationId,
            AuthenticityToken = currentUser.AuthenticityToken!,
            TwoFactorToken = currentUser.LastVerified == null ? currentUser.TwoFactorToken : null
        };

    }

    public async Task<User> VerifyTotp(VerifyUserDto verifyUserDto)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == verifyUserDto.UserId) ??
                   throw new Exception($"Invalid user id", new BadRequestException());
        if (user.AuthenticityToken != verifyUserDto.AuthenticityToken)
            throw new Exception($"Invalid authenticity token", new UnauthorizedException());
        // TODO: check if token is not expired

        var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorToken), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var valid = totp.VerifyTotp(verifyUserDto.OtpAttempt, out var _,
            window: VerificationWindow.RfcSpecifiedNetworkDelay);
        if (!valid) throw new Exception($"Invalid authenticator code", new UnauthorizedException());

        user.LastVerified = DateTime.Now;
        await dbContext.SaveChangesAsync();
        return user;
    }


    public TokenResponse RefreshToken(RefreshTokenDto refreshTokenApiDto)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(refreshTokenApiDto.AccessToken);
        var user = dbContext.Users.SingleOrDefault(x => x.Id.ToString() == principal.Claims.FirstOrDefault(c => c.Type == "Id")!.Value);
        if (user == null || user.RefreshToken != refreshTokenApiDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new Exception("Invalid refresh token", new BadRequestException());
        }

        var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        dbContext.SaveChanges();
        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public TokenResponse GetToken(User user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new("Id", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Type.ToString()),
            };

            user.RefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1);
            dbContext.SaveChanges();
            
            return new TokenResponse
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = user.RefreshToken
            };
        }
        catch
        {
            throw new Exception($"Failed to create tokens", new InternalServerException());
        }
    }
}