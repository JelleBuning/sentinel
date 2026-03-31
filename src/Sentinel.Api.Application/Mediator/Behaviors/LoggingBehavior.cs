using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Sentinel.Api.Application.Mediator.Behaviors;

public class LoggingBehavior<TMessage, TResponse>(ILogger<LoggingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        var messageName = typeof(TMessage).Name;
        logger.LogInformation("Handling {MessageName}", messageName);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(message, cancellationToken);
            stopwatch.Stop();

            logger.LogInformation("Handled {MessageName} in {ElapsedMilliseconds}ms", 
                messageName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(ex, "Error handling {MessageName} after {ElapsedMilliseconds}ms", 
                messageName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
