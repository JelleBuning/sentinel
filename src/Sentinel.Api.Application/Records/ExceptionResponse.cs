namespace Sentinel.Api.Application.Records;

public record ExceptionResponse(int StatusCode, Exception Exception);