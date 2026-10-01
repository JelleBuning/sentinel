using Mycelium.Api.IntegrationTests.Common;
using NUnit.Framework;

namespace Mycelium.Api.IntegrationTests.Organisation;

public class OrganisationTests
{
    [Test]
    public async Task Authorized_GetAll_ShouldReturnOK()
    {
        await using var scope = await new TestScope().AuthenticateAsUserAsync();
        
        var result = await scope.Client.GetAsync("/api/v1/organisations");
        
        result.ShouldBeOk();
    }

    [Test]
    public async Task UnAuthorized_GetAll_ShouldReturnUnauthorized()
    {
        await using var scope = new TestScope();
        var result = await scope.Client.GetAsync("/api/v1/organisations");
        result.ShouldBeUnauthorized();
    }
}