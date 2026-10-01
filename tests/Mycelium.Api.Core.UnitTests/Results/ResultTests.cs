using Mycelium.Api.Core.Results;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Results;

public class ResultTests
{
    [Test]
    public void Success_ShouldBeSuccessAndNotFailure()
    {
        var result = Result.Success();

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.IsFailure, Is.False);
        Assert.That(result.Error, Is.EqualTo(Error.None));
    }

    [Test]
    public void Failure_ShouldBeFailureAndNotSuccess()
    {
        var error = Error.NotFound("not found");

        var result = Result.Failure(error);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo(error));
    }

    [Test]
    public void GenericSuccess_ShouldExposeValue()
    {
        var result = Result.Success(42);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void GenericFailure_ShouldNotBeSuccess()
    {
        var error = Error.Validation("invalid");

        var result = Result.Failure<int>(error);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo(error));
    }

    [Test]
    public void GenericFailure_AccessingValue_ShouldThrow()
    {
        var result = Result.Failure<int>(Error.Validation("invalid"));

        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Test]
    public void ImplicitOperator_FromValue_ShouldCreateSuccessResult()
    {
        Result<string> result = "hello";

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("hello"));
    }
}
