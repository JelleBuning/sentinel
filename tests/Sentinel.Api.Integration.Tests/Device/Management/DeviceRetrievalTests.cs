using System.Net;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.WorkerService.Common.Extensions;

namespace Sentinel.Api.Integration.Tests.Device.Management;

public class DeviceRetrievalTests
{
    private ApiFixture _fixture = null!;

    [SetUp]
    public Task Setup()
    {
        _fixture = new ApiFixture();
        return Task.CompletedTask;
    }

    [TearDown]
    public async Task TearDown() => await _fixture.DisposeAsync();

    [Test]
    public async Task Unauthorized_GetDevices_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _fixture.CreateClient();

        // Act
        var result = await client.GetAsync("/devices");

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Authorized_GetDevices_ShouldReturnOK()
    {
        // Arrange
        using var client = _fixture.CreateAuthenticatedUser(out _);

        // Act
        var result = await client.GetAsync("/devices");

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Authorized_GetDevices_ShouldOnlyReturnOrganisationDevices()
    {
        // Arrange
        using var client = _fixture.CreateAuthenticatedUser(out var user);
        await using var dbContext = _fixture.GetDbContext();
        
        var organisation = dbContext.Organisations.Single(x => x.Id == user.OrganisationId);
        var authorizedDevice = new Domain.Entities.Device
        {
            OrganisationId = organisation.Id,
            Name = "AuthorizedDevice",
        };
        var unauthorizedDevice = new Domain.Entities.Device
        {
            OrganisationId = organisation.Id + 1, 
            Name = "UnauthorizedOrganisationDevice"
        };
        await _fixture.AddDeviceAsync(authorizedDevice);
        await _fixture.AddDeviceAsync(unauthorizedDevice);

        // Act
        var result = await client.GetAsync("/devices");
        var devices = await result.Content.DeserializeAsync<GetDevicesResponse>();

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(devices!.Devices.Count, Is.EqualTo(1));
        Assert.That(devices.Devices.Any(d => d.Name == authorizedDevice.Name), Is.True);
        Assert.That(devices.Devices.Any(d => d.Name == unauthorizedDevice.Name), Is.False);
    }
}