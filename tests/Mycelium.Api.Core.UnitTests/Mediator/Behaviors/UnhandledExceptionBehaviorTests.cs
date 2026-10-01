using Mediator;
using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Api.Core.Mediator.Behaviors;
using Mycelium.Api.Core.Results;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Mediator.Behaviors;

public class UnhandledExceptionBehaviorTests
{
    public record TestMessage : IMessage;

    [Test]
    public async Task Handle_WhenNextSucceeds_ShouldReturnResponse()
    {
        var behavior = new UnhandledExceptionBehavior<TestMessage, Result>(
            NullLogger<UnhandledExceptionBehavior<TestMessage, Result>>.Instance);

        var response = await behavior.Handle(new TestMessage(), (_, _) =>
            ValueTask.FromResult(Result.Success()), CancellationToken.None);

        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public async Task Handle_WhenNextThrows_ShouldReturnUnexpectedFailure()
    {
        var behavior = new UnhandledExceptionBehavior<TestMessage, Result>(
            NullLogger<UnhandledExceptionBehavior<TestMessage, Result>>.Instance);

        var response = await behavior.Handle(new TestMessage(), (_, _) =>
            throw new InvalidOperationException("boom"), CancellationToken.None);

        Assert.That(response.IsFailure, Is.True);
        Assert.That(response.Error.Type, Is.EqualTo(ErrorType.Unexpected));
    }

    [Test]
    public async Task Handle_WhenNextThrows_ForGenericResult_ShouldReturnUnexpectedFailure()
    {
        var behavior = new UnhandledExceptionBehavior<TestMessage, Result<int>>(
            NullLogger<UnhandledExceptionBehavior<TestMessage, Result<int>>>.Instance);

        var response = await behavior.Handle(new TestMessage(), (_, _) =>
            throw new InvalidOperationException("boom"), CancellationToken.None);

        Assert.That(response.IsFailure, Is.True);
        Assert.That(response.Error.Type, Is.EqualTo(ErrorType.Unexpected));
        Assert.Throws<InvalidOperationException>(() => _ = response.Value);
    }
}
