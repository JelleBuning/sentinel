using System.Security.Claims;
using Mycelium.Api.Devices.Tasks.Startup.Register.v1;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.Api.Devices.UnitTests.Tasks.Startup.Register.v1;

public class RegisterDeviceHandlerTests
{
    [Test]
    public async Task Handle_OrganisationNotFound_ReturnsNotFound()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var tokenGenerator = Substitute.For<ITokenGenerator>();
        var handler = new RegisterDeviceHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new RegisterDeviceCommand(Guid.NewGuid(), "MyPc"), CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_OrganisationFound_CreatesDeviceAndReturnsTokens()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var organisation = new Organisation { Hash = Guid.NewGuid() };
        dbContext.Organisations.Add(organisation);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GenerateRefreshToken().Returns("refresh-token");
        tokenGenerator.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>()).Returns("access-token");

        var handler = new RegisterDeviceHandler(dbContext, tokenGenerator);

        var result = await handler.Handle(new RegisterDeviceCommand(organisation.Hash, "MyPc"), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.OrganisationId, Is.EqualTo(organisation.Id));
        Assert.That(result.Value.AccessToken, Is.EqualTo("access-token"));
        Assert.That(result.Value.RefreshToken, Is.EqualTo("refresh-token"));
        Assert.That(dbContext.Devices.Single().Name, Is.EqualTo("MyPc"));
        Assert.That(dbContext.Devices.Single().RefreshToken, Is.EqualTo("refresh-token"));
    }

    [Test]
    public async Task Handle_OrganisationFound_GeneratesAccessTokenWithDeviceClaims()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var organisation = new Organisation { Hash = Guid.NewGuid() };
        dbContext.Organisations.Add(organisation);
        await dbContext.SaveChangesAsync();

        var tokenGenerator = Substitute.For<ITokenGenerator>();
        tokenGenerator.GenerateRefreshToken().Returns("refresh-token");
        tokenGenerator.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>()).Returns("access-token");

        var handler = new RegisterDeviceHandler(dbContext, tokenGenerator);
        await handler.Handle(new RegisterDeviceCommand(organisation.Hash, "MyPc"), CancellationToken.None);

        tokenGenerator.Received(1).GenerateAccessToken(Arg.Is<IEnumerable<Claim>>(claims =>
            claims.Any(c => c.Type == "Name" && c.Value == "MyPc") &&
            claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Device")));
    }
}
