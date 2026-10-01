using Mediator;
using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Api.Core.Mediator.Behaviors;
using Mycelium.Api.Core.Results;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Mediator.Behaviors;

public class LoggingBehaviorTests
{
    public record TestMessage : IMessage;

    [Test]
    public async Task Handle_ShouldCallNext_AndReturnItsResponse()
    {
        var behavior = new LoggingBehavior<TestMessage, Result>(
            NullLogger<LoggingBehavior<TestMessage, Result>>.Instance);
        var nextCalled = false;

        var response = await behavior.Handle(new TestMessage(), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success());
        }, CancellationToken.None);

        Assert.That(nextCalled, Is.True);
        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public void Handle_WhenNextThrows_ShouldPropagateException()
    {
        var behavior = new LoggingBehavior<TestMessage, Result>(
            NullLogger<LoggingBehavior<TestMessage, Result>>.Instance);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            behavior.Handle(new TestMessage(), (_, _) =>
                throw new InvalidOperationException("boom"), CancellationToken.None).AsTask());
    }
}
