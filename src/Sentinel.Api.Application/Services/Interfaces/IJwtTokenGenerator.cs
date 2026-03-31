using System.Security.Claims;

namespace Sentinel.Api.Application.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}
