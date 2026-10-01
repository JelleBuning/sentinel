using System.Security.Claims;
using Mycelium.Api.Devices.Common;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Common;

public class ClaimsPrincipalExtensionsTests
{
    [Test]
    public void GetId_SingleIdClaim_ReturnsParsedId()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Id", "42")]));

        var id = user.GetId();

        Assert.That(id, Is.EqualTo(42));
    }

    [Test]
    public void GetId_NoIdClaim_Throws()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Name", "someone")]));

        Assert.Throws<InvalidOperationException>(() => user.GetId());
    }

    [Test]
    public void GetId_DuplicateIdClaims_Throws()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Id", "1"), new Claim("Id", "2")]));

        Assert.Throws<InvalidOperationException>(() => user.GetId());
    }

    [Test]
    public void GetId_NonNumericIdClaim_Throws()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Id", "not-a-number")]));

        Assert.Throws<FormatException>(() => user.GetId());
    }
}
