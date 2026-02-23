using FluentValidation;
using Mediator;
using Microsoft.Extensions.Logging;
using Sentinel.Api.Application.Exceptions;

namespace Sentinel.Api.Application.Mediator.Behaviors;

public class UnhandledExceptionBehavior<TMessage, TResponse>(
    ILogger<UnhandledExceptionBehavior<TMessage, TResponse>> logger
) : IPipelineBehavior<TMessage, TResponse> where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Mediator.Exceptions.ValidationException ex)
        {
            throw new BadValidationRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled Exception for Request {MessageName} {@Message}", typeof(TMessage).Name, message);
            throw;
        }
    }
}

