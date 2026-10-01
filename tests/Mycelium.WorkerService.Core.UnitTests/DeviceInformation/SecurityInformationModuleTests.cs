using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.DeviceInformation;

public class SecurityInformationModuleTests
{
    [Test]
    public async Task Execute_RetrievesSecurityInformationAndSendsItToTheApi()
    {
        var retriever = Substitute.For<ISecurityInformationRetriever>();
        retriever.Retrieve().Returns(new SecurityDto { LastSecurityScanDto = new LastSecurityScanDto() });
        var apiService = MyceliumApiServiceFactory.CreateSucceeding(out var handler);
        var config = Substitute.For<IScheduledModuleConfig<SecurityInformationModule>>();
        var module = new SecurityInformationModule(NullLogger<SecurityInformationModule>.Instance, config, retriever, apiService);

        await module.Execute(CancellationToken.None);

        retriever.Received(1).Retrieve();
        Assert.That(handler.Requests, Has.Count.EqualTo(1));
        Assert.That(handler.Requests[0].RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/1/security"));
    }
}
