using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.DeviceInformation;

public class SoftwareInformationModuleTests
{
    [Test]
    public async Task Execute_RetrievesSoftwareInformationAndSendsItToTheApi()
    {
        var retriever = Substitute.For<ISoftwareInformationRetriever>();
        retriever.Retrieve().Returns([new SoftwareDto { Name = "Notepad++" }]);
        var apiService = MyceliumApiServiceFactory.CreateSucceeding(out var handler);
        var config = Substitute.For<IScheduledModuleConfig<SoftwareInformationModule>>();
        var module = new SoftwareInformationModule(NullLogger<SoftwareInformationModule>.Instance, config, retriever, apiService);

        await module.Execute(CancellationToken.None);

        retriever.Received(1).Retrieve();
        Assert.That(handler.Requests, Has.Count.EqualTo(1));
        Assert.That(handler.Requests[0].RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/1/software"));
    }
}
