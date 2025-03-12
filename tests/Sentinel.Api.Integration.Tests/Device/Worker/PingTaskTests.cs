using System.Net;
using NUnit.Framework;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.WorkerService.Common.Api.Extensions;

namespace Sentinel.Api.Integration.Tests.Device.Worker;

public class PingTaskTests
{
    private ApiFixture _fixture = null!;
    private Domain.Entities.Organisation _organisation = null!;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new ApiFixture();
        _organisation = await _fixture.AddOrganisationAsync(new Domain.Entities.Organisation{ Hash = Guid.NewGuid()});
    }

    [TearDown]
    public async Task TearDown() => await _fixture.DisposeAsync();

    [Test]
    public async Task Authorized_TaskExecution_ShouldReturnOk()
    {
        // Arrange
        var client = _fixture.CreateAuthenticatedDevice(_organisation.Hash, out var device);
        
        // Act    
        var result = await client.PostAsync($"/devices/{device.Id}/ping");
        
        // // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}