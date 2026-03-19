using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Domain.Entities;
using Sentinel.Api.Infrastructure.Exceptions;
using Sentinel.Api.Infrastructure.Persistence;

namespace Sentinel.Api.Infrastructure.Repositories;

public class AuthRepository(AppDbContext dbContext, IConfiguration configuration) : IAuthRepository
{
    
    public async Task<SignInUserResponse> AuthenticateAsync(SignInUserDto user)
    {
        var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(user.Email.ToLower()));
        if (currentUser == null) throw new UnauthorizedException("Invalid login credentials");
        
        var passwordValid = BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password);
        if (!passwordValid) throw new UnauthorizedException("Invalid login credentials");
            
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
        };

        currentUser.AuthenticityToken = GenerateAccessToken(claims);
        await dbContext.SaveChangesAsync();
        
        return new SignInUserResponse
        {
            UserId = currentUser.Id,
            OrganisationId = currentUser.OrganisationId,
            AuthenticityToken = currentUser.AuthenticityToken!,
            TwoFactorToken = currentUser.LastVerified == null ? currentUser.TwoFactorToken : null
        };

    }

    public async Task<TokenDto> GetTokenAsync(User user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new("Id", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Type.ToString()),
            };

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1);
            await dbContext.SaveChangesAsync();
            
            return new TokenDto
            {
                AccessToken = GenerateAccessToken(claims),
                RefreshToken = user.RefreshToken
            };
        }
        catch
        {
            throw new InternalServerException($"Failed to create tokens");
        }
    }

    public async Task<User> VerifyTotpAsync(VerifyUserDto verifyUserDto)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == verifyUserDto.UserId) ??
                   throw new BadRequestException($"Invalid user id");
        if (user.AuthenticityToken != verifyUserDto.AuthenticityToken)
            throw new UnauthorizedException($"Invalid authenticity token");
        // TODO: check if token is not expired

        var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorToken), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var valid = totp.VerifyTotp(verifyUserDto.OtpAttempt, out var _,
            window: VerificationWindow.RfcSpecifiedNetworkDelay);
        if (!valid) throw new UnauthorizedException($"Invalid authenticator code");

        user.LastVerified = DateTime.Now;
        await dbContext.SaveChangesAsync();
        return user;
    }
    
    public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var claimId = principal.Claims.SingleOrDefault(c => c.Type == "Id")!.Value;
        var role = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role)!.Value;
        
        var newAccessToken = GenerateAccessToken(principal.Claims);
        var newRefreshToken = GenerateRefreshToken();
        
        switch (role)
        {
            case "Admin":
            case "User":
                var user = dbContext.Users.SingleOrDefault(x => x.Id.ToString() == claimId);
                if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    throw new BadRequestException("Invalid refresh token");
                }
                user.RefreshToken = newRefreshToken;
                break;
            case "Device":
                var device = dbContext.Devices.SingleOrDefault(x => x.Id.ToString() == claimId);
                if (device == null || device.RefreshToken != tokenDto.RefreshToken)
                {
                    throw new BadRequestException("Invalid refresh token");
                }
                device.RefreshToken = newRefreshToken;
                break;
            default:
                throw new Exception("Invalid role");
        }

        await dbContext.SaveChangesAsync();
        return new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
    
    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(3),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}