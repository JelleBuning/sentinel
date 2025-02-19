using System.Net;
using System.Text.Json;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.Device.Management;

public class DeviceRetrievalTests
{
    private ApiFixture _fixture;

    [SetUp]
    public Task Setup()
    {
        _fixture  = new ApiFixture();
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
        using var client = _fixture.CreateAuthenticatedClient(out _);
        
        // Act
        var result = await client.GetAsync("/devices");
        
        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [Test]
    public async Task Authorized_GetDevices_ShouldOnlyReturnOrganisationDevices()
    {
        // Arrange
        using var client = _fixture.CreateAuthenticatedClient(out var user);
        var authorizedDevice = new Domain.Entities.Device
        {
            OrganisationId = user.OrganisationId,
            Name = "AuthorizedDevice"
        };
        var unauthorizedDevice = new Domain.Entities.Device
        {
            OrganisationId = user.OrganisationId + 1,
            Name = "OtherOrganisationDevice",
        };
        await _fixture.AddDeviceAsync(authorizedDevice);
        await _fixture.AddDeviceAsync(unauthorizedDevice);
        
        // Act
        var result = await client.GetAsync("/devices");
        var devices = await JsonSerializer.DeserializeAsync<GetDevicesResponse>(await result.Content.ReadAsStreamAsync());
        
        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(devices!.Devices.Count, Is.EqualTo(1));
        Assert.That(devices.Devices.Any(d => d.Name == authorizedDevice.Name), Is.True);
        Assert.That(devices.Devices.Any(d => d.Name == unauthorizedDevice.Name), Is.False);
    }
}