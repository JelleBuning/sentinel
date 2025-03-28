using System.Net;
using NUnit.Framework;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Integration.Tests.Common;
using Sentinel.WorkerService.Common.Api.Extensions;

namespace Sentinel.Api.Integration.Tests.Device.Authentication;

public class RegisterTests
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
    public async Task Correct_Registration_ShouldReturnOK()
    {
        // Arrange
        using var client = _fixture.CreateClient();

        // Act
        var result = await client.PostAsync("/devices/register", new RegisterDeviceDto
        {
            Name = "John Doe",
            OrganisationHash = _organisation.Hash,
        });

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task InvalidOrganisationHash_Registration_ShouldReturnNotFound()
    {
        // Arrange
        using var client = _fixture.CreateClient();

        // Act
        var result = await client.PostAsync("/devices/register", new RegisterDeviceDto
        {
            Name = "John Doe",
            OrganisationHash = Guid.Empty,
        });

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}