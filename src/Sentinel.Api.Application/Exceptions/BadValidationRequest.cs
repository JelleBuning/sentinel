using Sentinel.Api.Infrastructure.Exceptions;

namespace Sentinel.Api.Application.Exceptions;

public class BadValidationRequest(string message) : DomainException(message)
{
    
}