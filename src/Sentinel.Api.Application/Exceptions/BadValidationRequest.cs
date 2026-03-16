namespace Sentinel.Api.Application.Exceptions;

public class BadValidationRequest(string message) : Exception(message);
