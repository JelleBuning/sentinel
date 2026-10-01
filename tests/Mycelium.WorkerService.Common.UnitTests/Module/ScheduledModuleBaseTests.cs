using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.WorkerService.Common.Module;
using Mycelium.WorkerService.Common.Module.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Common.UnitTests.Module;

public class ScheduledModuleBaseTests
{
    [Test]
    public async Task StartAsync_RunImmediately_ExecutesRightAway()
    {
        var executed = false;
        var config = Substitute.For<IScheduledModuleConfig<TestScheduledModule>>();
        config.Interval.Returns(TimeSpan.FromMilliseconds(50));
        var module = new TestScheduledModule(NullLogger<TestScheduledModule>.Instance, config, runImmediately: true, _ =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        // Cancelled token prevents the finally-block reschedule from leaving a background timer running.
        await module.StartAsync(new CancellationToken(canceled: true));

        Assert.That(executed, Is.True);
    }

    [Test]
    public async Task StartAsync_RunImmediately_ExecuteThrows_ExceptionIsCaughtAndLogged()
    {
        var config = Substitute.For<IScheduledModuleConfig<TestScheduledModule>>();
        config.Interval.Returns(TimeSpan.FromMilliseconds(50));
        var module = new TestScheduledModule(NullLogger<TestScheduledModule>.Instance, config, runImmediately: true,
            _ => throw new InvalidOperationException("boom"));

        Assert.DoesNotThrowAsync(async () => await module.StartAsync(new CancellationToken(canceled: true)));
    }

    [Test]
    public async Task StartAsync_NotRunImmediately_DoesNotExecuteBeforeIntervalElapses()
    {
        var executed = false;
        var config = Substitute.For<IScheduledModuleConfig<TestScheduledModule>>();
        config.Interval.Returns(TimeSpan.FromMinutes(10));
        var module = new TestScheduledModule(NullLogger<TestScheduledModule>.Instance, config, runImmediately: false, _ =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        await module.StartAsync(CancellationToken.None);

        Assert.That(executed, Is.False);

        await module.StopAsync(CancellationToken.None);
    }

    public sealed class TestScheduledModule(
        Microsoft.Extensions.Logging.ILogger<TestScheduledModule> logger,
        IScheduledModuleConfig<TestScheduledModule> config,
        bool runImmediately,
        Func<CancellationToken, Task> onExecute)
        : ScheduledModuleBase<TestScheduledModule>(logger, config, runImmediately)
    {
        public override Task Execute(CancellationToken cancellationToken) => onExecute(cancellationToken);
    }
}
