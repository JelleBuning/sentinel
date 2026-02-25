namespace Sentinel.Api.Infrastructure.Exceptions;

public class UnauthorizedException(string message) : DomainException(message);