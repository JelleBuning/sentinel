using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Mycelium.Api.Core.Results;
using NUnit.Framework;

namespace Mycelium.Api.Core.UnitTests.Results;

public class ResultHttpExtensionsTests
{
    [Test]
    public void ToHttpResult_OnSuccess_ShouldReturnOk()
    {
        var result = Result.Success();

        var httpResult = result.ToHttpResult();

        Assert.That(httpResult, Is.InstanceOf<Ok>());
    }

    [Test]
    public void ToHttpResult_Generic_OnSuccess_ShouldReturnOkWithValue()
    {
        var result = Result.Success(42);

        var httpResult = result.ToHttpResult();

        Assert.That(httpResult, Is.InstanceOf<Ok<int>>());
        Assert.That(((Ok<int>)httpResult).Value, Is.EqualTo(42));
    }

    [TestCase(ErrorType.Validation, StatusCodes.Status400BadRequest)]
    [TestCase(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [TestCase(ErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    [TestCase(ErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    [TestCase(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    [TestCase(ErrorType.Unexpected, StatusCodes.Status500InternalServerError)]
    public void ToHttpResult_OnFailure_ShouldMapErrorTypeToStatusCode(ErrorType errorType, int expectedStatusCode)
    {
        var error = new Error(errorType, "message");

        var httpResult = error.ToHttpResult();

        var statusCodeResult = (IStatusCodeHttpResult)httpResult;
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(expectedStatusCode));
    }

    [Test]
    public void ToHttpResult_OnFailure_ShouldMapErrorTypeToStatusCode_ForPlainResult()
    {
        var result = Result.Failure(Error.NotFound("missing"));

        var httpResult = result.ToHttpResult();

        var statusCodeResult = (IStatusCodeHttpResult)httpResult;
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
    }

    [Test]
    public void ToHttpResult_Generic_OnFailure_ShouldMapErrorTypeToStatusCode()
    {
        var result = Result.Failure<int>(Error.Forbidden("nope"));

        var httpResult = result.ToHttpResult();

        var statusCodeResult = (IStatusCodeHttpResult)httpResult;
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
    }
}
