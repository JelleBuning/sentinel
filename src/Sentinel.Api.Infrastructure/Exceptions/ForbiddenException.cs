namespace Sentinel.Api.Infrastructure.Exceptions;

public class ForbiddenException(string message) : DomainException(message);