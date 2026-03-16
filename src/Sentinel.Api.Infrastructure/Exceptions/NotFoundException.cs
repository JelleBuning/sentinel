namespace Sentinel.Api.Infrastructure.Exceptions;

public class NotFoundException(string message) : Exception(message);