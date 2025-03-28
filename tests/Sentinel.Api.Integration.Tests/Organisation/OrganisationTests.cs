using System.Net;
using NUnit.Framework;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.Organisation;

public class OrganisationTests
{
    private ApiFixture _fixture = null!;

    [SetUp]
    public Task Setup()
    {
        _fixture  = new ApiFixture();
        return Task.CompletedTask;
    }
    
    [TearDown]
    public async Task TearDown() => await _fixture.DisposeAsync();
    
    [Test]
    public async Task Authorized_GetAll_ShouldReturnOK()
    {
        // Arrange
        using var client = _fixture.CreateAuthenticatedUser(out _);
        
        // Act
        var result = await client.GetAsync("/organisations");
        
        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UnAuthorized_GetAll_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _fixture.CreateClient();

        // Act
        var result = await client.GetAsync("/organisations");
        
        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}