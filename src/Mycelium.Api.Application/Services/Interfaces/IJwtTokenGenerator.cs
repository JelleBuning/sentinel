using System.Security.Claims;

namespace Mycelium.Api.Application.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}
