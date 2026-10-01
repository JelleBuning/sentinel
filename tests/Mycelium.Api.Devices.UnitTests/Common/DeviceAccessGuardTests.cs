using System.Security.Claims;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Common;

public class DeviceAccessGuardTests
{
    [Test]
    public void Validate_NullUser_ReturnsUnauthorized()
    {
        var error = DeviceAccessGuard.Validate(null, 1);

        Assert.That(error, Is.Not.Null);
        Assert.That(error!.Type, Is.EqualTo(ErrorType.Unauthorized));
    }

    [Test]
    public void Validate_MissingIdClaim_ReturnsForbidden()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Name", "device")]));

        var error = DeviceAccessGuard.Validate(user, 1);

        Assert.That(error, Is.Not.Null);
        Assert.That(error!.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public void Validate_NonNumericIdClaim_ReturnsForbidden()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Id", "not-a-number")]));

        var error = DeviceAccessGuard.Validate(user, 1);

        Assert.That(error, Is.Not.Null);
        Assert.That(error!.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public void Validate_IdClaimDoesNotMatchTargetDevice_ReturnsForbidden()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Id", "2")]));

        var error = DeviceAccessGuard.Validate(user, 1);

        Assert.That(error, Is.Not.Null);
        Assert.That(error!.Type, Is.EqualTo(ErrorType.Forbidden));
    }

    [Test]
    public void Validate_IdClaimMatchesTargetDevice_ReturnsNull()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Id", "1")]));

        var error = DeviceAccessGuard.Validate(user, 1);

        Assert.That(error, Is.Null);
    }
}
