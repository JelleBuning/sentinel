using Mycelium.Api.Core.Results;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Results;

public class ResultFactoryTests
{
    [Test]
    public void Failure_ForPlainResult_ShouldReturnFailedResult()
    {
        var error = Error.Unexpected("boom");

        var result = ResultFactory<Result>.Failure(error);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo(error));
    }

    [Test]
    public void Failure_ForGenericResult_ShouldReturnFailedResultOfCorrectType()
    {
        var error = Error.NotFound("missing");

        var result = ResultFactory<Result<int>>.Failure(error);

        Assert.That(result, Is.InstanceOf<Result<int>>());
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo(error));
    }

    [Test]
    public void Failure_ForGenericResult_AccessingValue_ShouldThrow()
    {
        var result = ResultFactory<Result<string>>.Failure(Error.Conflict("conflict"));

        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Test]
    public void Failure_IsCachedPerClosedGenericType()
    {
        var first = ResultFactory<Result<int>>.Failure;
        var second = ResultFactory<Result<int>>.Failure;

        Assert.That(first, Is.SameAs(second));
    }
}
