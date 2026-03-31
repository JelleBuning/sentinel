namespace Sentinel.Api.Infrastructure.Exceptions;

public class InternalServerException(string message) : Exception(message);