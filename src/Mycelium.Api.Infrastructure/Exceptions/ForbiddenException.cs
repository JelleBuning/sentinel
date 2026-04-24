namespace Mycelium.Api.Infrastructure.Exceptions;

public class ForbiddenException(string message) : Exception(message);