using Mycelium.Api.Core.Results;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Results;

public class ErrorTests
{
    [Test]
    public void None_ShouldHaveNoneTypeAndEmptyMessage()
    {
        Assert.That(Error.None.Type, Is.EqualTo(ErrorType.None));
        Assert.That(Error.None.Message, Is.EqualTo(string.Empty));
    }

    [TestCase(ErrorType.Validation)]
    [TestCase(ErrorType.NotFound)]
    [TestCase(ErrorType.Unauthorized)]
    [TestCase(ErrorType.Forbidden)]
    [TestCase(ErrorType.Conflict)]
    [TestCase(ErrorType.Unexpected)]
    public void FactoryMethods_ShouldProduceMatchingErrorType(ErrorType expectedType)
    {
        var error = expectedType switch
        {
            ErrorType.Validation => Error.Validation("message"),
            ErrorType.NotFound => Error.NotFound("message"),
            ErrorType.Unauthorized => Error.Unauthorized("message"),
            ErrorType.Forbidden => Error.Forbidden("message"),
            ErrorType.Conflict => Error.Conflict("message"),
            ErrorType.Unexpected => Error.Unexpected("message"),
            _ => throw new ArgumentOutOfRangeException(nameof(expectedType))
        };

        Assert.That(error.Type, Is.EqualTo(expectedType));
        Assert.That(error.Message, Is.EqualTo("message"));
    }

    [Test]
    public void Errors_WithSameTypeAndMessage_ShouldBeEqual()
    {
        var first = Error.NotFound("missing");
        var second = Error.NotFound("missing");

        Assert.That(first, Is.EqualTo(second));
    }
}
