using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.DeviceInformation;

public class DiskInformationModuleTests
{
    [Test]
    public async Task Execute_RetrievesDisksAndSendsThemToTheApi()
    {
        var retriever = Substitute.For<IDiskInformationRetriever>();
        retriever.Retrieve().Returns([new DiskDto { Name = "C:" }]);
        var apiService = MyceliumApiServiceFactory.CreateSucceeding(out var handler);
        var config = Substitute.For<IScheduledModuleConfig<DiskInformationModule>>();
        var module = new DiskInformationModule(NullLogger<DiskInformationModule>.Instance, config, retriever, apiService);

        await module.Execute(CancellationToken.None);

        retriever.Received(1).Retrieve();
        Assert.That(handler.Requests, Has.Count.EqualTo(1));
        Assert.That(handler.Requests[0].RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/1/disks"));
    }
}
