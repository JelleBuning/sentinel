namespace Sentinel.Api.Infrastructure.Exceptions;

public class NotFoundException(string message) : DomainException(message);