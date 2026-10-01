using Mycelium.Api.IntegrationTests.Common;
using Mycelium.Common.DTO.Device;
using NUnit.Framework;

namespace Mycelium.Api.IntegrationTests.Device.Authentication;

public class RegisterTests
{
    [Test]
    public async Task Correct_Registration_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsUserAsync();

        var result = await scope.Client.PostAsync("/api/v1/devices/register", new RegisterDeviceRequest
        {
            Name = "John Doe",
            OrganisationHash = scope.Organisation.Hash,
        });

        result.ShouldBeOk();
    }

    [Test]
    public async Task EmptyOrganisationHash_Registration_ShouldReturnBadRequest()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsUserAsync();

        var result = await scope.Client.PostAsync("/api/v1/devices/register", new RegisterDeviceRequest
        {
            Name = "John Doe",
            OrganisationHash = Guid.Empty,
        });

        result.ShouldBeBadRequest();
    }

    [Test]
    public async Task UnknownOrganisationHash_Registration_ShouldReturnNotFound()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsUserAsync();

        var result = await scope.Client.PostAsync("/api/v1/devices/register", new RegisterDeviceRequest
        {
            Name = "John Doe",
            OrganisationHash = Guid.NewGuid(),
        });

        result.ShouldBeNotFound();
    }
}
