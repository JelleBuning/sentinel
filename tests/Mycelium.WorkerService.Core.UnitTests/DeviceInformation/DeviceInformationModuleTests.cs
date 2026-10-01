using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.DTO.Device.Information;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;
using Mycelium.WorkerService.Core.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.UnitTests.DeviceInformation;

public class DeviceInformationModuleTests
{
    [Test]
    public async Task Execute_RetrievesInformationAndSendsItToTheApi()
    {
        var retriever = Substitute.For<IDeviceInformationRetriever>();
        var informationDto = new InformationDto();
        retriever.Retrieve().Returns(informationDto);
        var apiService = MyceliumApiServiceFactory.CreateSucceeding(out var handler);
        var config = Substitute.For<IScheduledModuleConfig<DeviceInformationModule>>();
        var module = new DeviceInformationModule(NullLogger<DeviceInformationModule>.Instance, config, retriever, apiService);

        await module.Execute(CancellationToken.None);

        retriever.Received(1).Retrieve();
        Assert.That(handler.Requests, Has.Count.EqualTo(1));
        Assert.That(handler.Requests[0].RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/1"));
    }
}
