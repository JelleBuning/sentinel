using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Mycelium.Api.Core.Mediator.Behaviors;
using Mycelium.Api.Core.Results;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Mediator.Behaviors;

public class ValidationBehaviorTests
{
    public record TestMessage(string Name) : IMessage;

    [Test]
    public async Task Handle_WithNoValidators_ShouldCallNext()
    {
        var behavior = new ValidationBehavior<TestMessage, Result>([]);
        var nextCalled = false;

        var response = await behavior.Handle(new TestMessage("value"), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success());
        }, CancellationToken.None);

        Assert.That(nextCalled, Is.True);
        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public async Task Handle_WithNoValidationFailures_ShouldCallNext()
    {
        var validator = Substitute.For<IValidator<TestMessage>>();
        validator.Validate(Arg.Any<ValidationContext<TestMessage>>()).Returns(new ValidationResult());
        var behavior = new ValidationBehavior<TestMessage, Result>([validator]);
        var nextCalled = false;

        var response = await behavior.Handle(new TestMessage("value"), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success());
        }, CancellationToken.None);

        Assert.That(nextCalled, Is.True);
        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public async Task Handle_WithValidationFailures_ShouldReturnValidationFailure_WithoutCallingNext()
    {
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };
        var validator = Substitute.For<IValidator<TestMessage>>();
        validator.Validate(Arg.Any<ValidationContext<TestMessage>>()).Returns(new ValidationResult(failures));
        var behavior = new ValidationBehavior<TestMessage, Result>([validator]);
        var nextCalled = false;

        var response = await behavior.Handle(new TestMessage(""), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success());
        }, CancellationToken.None);

        Assert.That(nextCalled, Is.False);
        Assert.That(response.IsFailure, Is.True);
        Assert.That(response.Error.Type, Is.EqualTo(ErrorType.Validation));
        Assert.That(response.Error.Message, Is.EqualTo("Name is required"));
    }

    [Test]
    public async Task Handle_WithMultipleValidatorFailures_ShouldJoinMessages()
    {
        var validator1 = Substitute.For<IValidator<TestMessage>>();
        validator1.Validate(Arg.Any<ValidationContext<TestMessage>>())
            .Returns(new ValidationResult([new ValidationFailure("Name", "error one")]));
        var validator2 = Substitute.For<IValidator<TestMessage>>();
        validator2.Validate(Arg.Any<ValidationContext<TestMessage>>())
            .Returns(new ValidationResult([new ValidationFailure("Name", "error two")]));
        var behavior = new ValidationBehavior<TestMessage, Result>([validator1, validator2]);

        var response = await behavior.Handle(new TestMessage(""), (_, _) =>
            ValueTask.FromResult(Result.Success()), CancellationToken.None);

        Assert.That(response.Error.Message, Is.EqualTo("error one error two"));
    }
}
