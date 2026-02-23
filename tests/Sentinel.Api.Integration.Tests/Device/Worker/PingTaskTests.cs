using NUnit.Framework;
using Sentinel.Api.Integration.Tests.Common;

namespace Sentinel.Api.Integration.Tests.Device.Worker;

public class PingTaskTests
{
    [Test]
    public async Task Authorized_TaskExecution_ShouldReturnOk()
    {
        await using var scope = new TestScope();
        await scope.AuthenticateAsDeviceAsync();
      
        var device = scope.DbContext.Devices.Single(x => x.OrganisationId == scope.Organisation.Id);
        
        var result = await scope.Client.PostAsync($"/devices/{device.Id}/ping");
        result.ShouldBeOk();
    }
    
    [Test]
    public async Task UnAuthorized_TaskExecution_ShouldReturnUnauthorized()
    {
        var scope = new TestScope();
        var result = await scope.Client.PostAsync($"/devices/1/ping");
        result.ShouldBeUnauthorized();
    }
}