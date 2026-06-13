namespace Mycelium.Api.Application.Exceptions;

public class BadValidationRequest(string message) : Exception(message);
