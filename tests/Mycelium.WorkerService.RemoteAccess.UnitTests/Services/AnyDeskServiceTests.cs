using System.ServiceProcess;
using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.RemoteAccess.Services;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.RemoteAccess.UnitTests.Services;

#pragma warning disable CA1416
public class AnyDeskServiceTests
{
    [Test]
    public void Start_ServiceAlreadyRunning_DoesNotCallStart()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var processHandle = Substitute.For<IProcessHandle>();
        processHandle.ReadLine().Returns("connection-id-1");
        processRunner.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(processHandle);

        var serviceController = Substitute.For<IServiceControllerWrapper>();
        serviceController.Status.Returns(ServiceControllerStatus.Running);

        var service = new AnyDeskService(processRunner, serviceController);

        var result = service.Start();

        serviceController.DidNotReceive().Start();
        serviceController.Received(1).WaitForStatus(ServiceControllerStatus.Running, Arg.Any<TimeSpan>());
        Assert.That(result.Id, Is.EqualTo("connection-id-1"));
    }

    [Test]
    public void Start_ServiceStopped_CallsStartBeforeWaiting()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var processHandle = Substitute.For<IProcessHandle>();
        processHandle.ReadLine().Returns("connection-id-2");
        processRunner.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(processHandle);

        var serviceController = Substitute.For<IServiceControllerWrapper>();
        serviceController.Status.Returns(ServiceControllerStatus.Stopped);

        var service = new AnyDeskService(processRunner, serviceController);

        var result = service.Start();

        serviceController.Received(1).Start();
        serviceController.Received(1).WaitForStatus(ServiceControllerStatus.Running, Arg.Any<TimeSpan>());
        Assert.That(result.Id, Is.EqualTo("connection-id-2"));
    }

    [Test]
    public void Start_NoIdReturned_Throws()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var processHandle = Substitute.For<IProcessHandle>();
        processHandle.ReadLine().Returns((string?)null);
        processRunner.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(processHandle);

        var serviceController = Substitute.For<IServiceControllerWrapper>();
        serviceController.Status.Returns(ServiceControllerStatus.Running);

        var service = new AnyDeskService(processRunner, serviceController);

        var ex = Assert.Throws<Exception>(() => service.Start())!;
        Assert.That(ex.Message, Is.EqualTo("WindowsRemoteAccess id not found"));
    }

    [Test]
    public void Stop_ServiceAlreadyStopped_DoesNothing()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var serviceController = Substitute.For<IServiceControllerWrapper>();
        serviceController.Status.Returns(ServiceControllerStatus.Stopped);

        var service = new AnyDeskService(processRunner, serviceController);

        service.Stop();

        serviceController.DidNotReceive().Stop();
        serviceController.DidNotReceive().WaitForStatus(Arg.Any<ServiceControllerStatus>(), Arg.Any<TimeSpan>());
    }

    [Test]
    public void Stop_ServiceRunning_StopsAndWaits()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var serviceController = Substitute.For<IServiceControllerWrapper>();
        serviceController.Status.Returns(ServiceControllerStatus.Running);

        var service = new AnyDeskService(processRunner, serviceController);

        service.Stop();

        serviceController.Received(1).Stop();
        serviceController.Received(1).WaitForStatus(ServiceControllerStatus.Stopped, Arg.Any<TimeSpan>());
    }

    [TestCase(ServiceControllerStatus.Running, true)]
    [TestCase(ServiceControllerStatus.Stopped, false)]
    public void IsRunning_ReflectsControllerStatus(ServiceControllerStatus status, bool expected)
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var serviceController = Substitute.For<IServiceControllerWrapper>();
        serviceController.Status.Returns(status);

        var service = new AnyDeskService(processRunner, serviceController);

        Assert.That(service.IsRunning, Is.EqualTo(expected));
    }
}
