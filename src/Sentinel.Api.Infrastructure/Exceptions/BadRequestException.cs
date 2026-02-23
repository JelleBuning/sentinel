namespace Sentinel.Api.Infrastructure.Exceptions;

public class BadRequestException(string message) : DomainException(message);