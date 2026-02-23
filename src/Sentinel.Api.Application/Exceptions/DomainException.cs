namespace Sentinel.Api.Infrastructure.Exceptions;

public class DomainException(string message) : Exception(message);