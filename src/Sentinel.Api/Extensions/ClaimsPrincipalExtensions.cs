using System.Security.Claims;

namespace Sentinel.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetId(this ClaimsPrincipal identity)
    {
        var idClaim = identity.Claims.Single(x => x.Type == "Id").Value;
        return int.Parse(idClaim);
    }
}