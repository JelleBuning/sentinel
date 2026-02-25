using NUnit.Framework;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.Device.Authentication;

public class RegisterTests
{
    [Test]
    public async Task Correct_Registration_ShouldReturnOK()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsUserAsync();
        
        var result = await scope.Client.PostAsync("/devices/register", new RegisterDeviceDto
        {
            Name = "John Doe",
            OrganisationHash = scope.Organisation.Hash,
        });

        result.ShouldBeOk();
    }

    [Test]
    public async Task InvalidOrganisationHash_Registration_ShouldReturnNotFound()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsUserAsync();
        
        var result = await scope.Client.PostAsync("/devices/register", new RegisterDeviceDto
        {
            Name = "John Doe",
            OrganisationHash = Guid.Empty,
        });

        result.ShouldBeBadRequest();
    }
}