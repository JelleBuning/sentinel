using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mycelium.Api.Core.Security;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Security;

public class TokenGeneratorTests
{
    private static TokenGenerator CreateGenerator(string key = "this-is-a-sufficiently-long-test-signing-key-123")
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = key,
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            })
            .Build();

        return new TokenGenerator(configuration);
    }

    [Test]
    public void GenerateAccessToken_ShouldProduceNonEmptyToken()
    {
        var generator = CreateGenerator();

        var token = generator.GenerateAccessToken([new Claim(ClaimTypes.NameIdentifier, "1")]);

        Assert.That(token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void GenerateRefreshToken_ShouldProduceDifferentValuesEachCall()
    {
        var generator = CreateGenerator();

        var first = generator.GenerateRefreshToken();
        var second = generator.GenerateRefreshToken();

        Assert.That(first, Is.Not.EqualTo(second));
    }

    [Test]
    public void GetPrincipalFromExpiredToken_ShouldReturnPrincipalWithOriginalClaims()
    {
        var generator = CreateGenerator();
        var token = generator.GenerateAccessToken([new Claim(ClaimTypes.NameIdentifier, "42")]);

        var principal = generator.GetPrincipalFromExpiredToken(token);

        Assert.That(principal.FindFirstValue(ClaimTypes.NameIdentifier), Is.EqualTo("42"));
    }

    [Test]
    public void GetPrincipalFromExpiredToken_WithTamperedToken_ShouldThrow()
    {
        var generator = CreateGenerator();
        var token = generator.GenerateAccessToken([new Claim(ClaimTypes.NameIdentifier, "42")]);
        var tampered = token[..^2] + (token[^2] == 'A' ? "B" : "A") + token[^1];

        Assert.Catch<SecurityTokenException>(() => generator.GetPrincipalFromExpiredToken(tampered));
    }

    [Test]
    public void GetPrincipalFromExpiredToken_SignedWithDifferentKey_ShouldThrow()
    {
        var generator = CreateGenerator();
        var otherGenerator = CreateGenerator("a-completely-different-signing-key-9876543210");
        var token = otherGenerator.GenerateAccessToken([new Claim(ClaimTypes.NameIdentifier, "42")]);

        Assert.Catch<SecurityTokenException>(() => generator.GetPrincipalFromExpiredToken(token));
    }
}
