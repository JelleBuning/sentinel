using System.Net;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.Device.Authentication;

public class RegisterTests
{
    private ApiFixture _fixture = null!;
    private Domain.Entities.Organisation _organisation;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new ApiFixture();
        _organisation = await _fixture.AddOrganisationAsync(new Domain.Entities.Organisation{ Hash = Guid.NewGuid()});
    }

    [TearDown]
    public async Task TearDown() => await _fixture.DisposeAsync();

    [Test]
    public async Task Correct_Registration_ShouldReturnOK()
    {
        // Arrange
        using var client = _fixture.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(new RegisterDeviceDto()
        {
            Name = "John Doe",
            OrganisationHash = _organisation.Hash,
        }), Encoding.UTF8, "application/json");
        var result = await client.PostAsync("/devices/auth/register", content);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task InvalidOrganisationHash_Registration_ShouldReturnNotFound()
    {
        // Arrange
        using var client = _fixture.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(new RegisterDeviceDto()
        {
            Name = "John Doe",
            OrganisationHash = Guid.Empty,
        }), Encoding.UTF8, "application/json");
        var result = await client.PostAsync("/devices/auth/register", content);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}