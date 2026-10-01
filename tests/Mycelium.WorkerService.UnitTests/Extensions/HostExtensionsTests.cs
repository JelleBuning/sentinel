using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.UnitTests.Extensions;

public class HostExtensionsTests
{
    [Test]
    public void ExecuteStartupModules_AllModulesSucceed_ExecutesEachOne()
    {
        var services = new ServiceCollection();
        var first = Substitute.For<IStartupModule>();
        var second = Substitute.For<IStartupModule>();
        first.Execute(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        second.Execute(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        services.AddSingleton(first);
        services.AddSingleton(second);

        var host = Substitute.For<IHost>();
        host.Services.Returns(services.BuildServiceProvider());

        var result = host.ExecuteStartupModules();

        first.Received(1).Execute(Arg.Any<CancellationToken>());
        second.Received(1).Execute(Arg.Any<CancellationToken>());
        Assert.That(result, Is.SameAs(host));
    }

    [Test]
    public void ExecuteStartupModules_OneModuleThrowsSynchronously_StillExecutesTheOthers()
    {
        var services = new ServiceCollection();
        var throwing = new ThrowingStartupModule();
        var recording = new RecordingStartupModule();
        services.AddSingleton<IStartupModule>(throwing);
        services.AddSingleton<IStartupModule>(recording);

        var host = Substitute.For<IHost>();
        host.Services.Returns(services.BuildServiceProvider());

        Assert.DoesNotThrow(() => host.ExecuteStartupModules());
        Assert.That(recording.Executed, Is.True);
    }

    private sealed class ThrowingStartupModule : IStartupModule
    {
        public Task Execute(CancellationToken cancellationToken) => throw new InvalidOperationException("boom");
    }

    private sealed class RecordingStartupModule : IStartupModule
    {
        public bool Executed { get; private set; }

        public Task Execute(CancellationToken cancellationToken)
        {
            Executed = true;
            return Task.CompletedTask;
        }
    }
}
