using Mediator;
using Microsoft.Extensions.Logging;

namespace CartMate.Application.Common.Behaviors;

/// <summary>
/// Logs each message's name and duration. Deliberately does NOT log message contents:
/// commands contain emails and passwords, which must never reach the logs.
/// </summary>
public sealed partial class LoggingBehavior<TMessage, TResponse>(
    ILogger<LoggingBehavior<TMessage, TResponse>> logger,
    TimeProvider time)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var messageName = typeof(TMessage).Name;
        var started = time.GetTimestamp();

        try
        {
            var response = await next(message, cancellationToken);
            if (logger.IsEnabled(LogLevel.Information))
            {
                // LogHandled(logger, messageName, time.GetElapsedTime(started).TotalMilliseconds);
                var elapsedMs = time.GetElapsedTime(started).TotalMilliseconds;
                LogHandled(logger, messageName, elapsedMs);
            }

            return response;
        }
        catch (Exception ex)
        {
            // LogFailed(logger, ex, messageName, time.GetElapsedTime(started).TotalMilliseconds);
            if (logger.IsEnabled(LogLevel.Warning))
            {
                var elapsedMs = time.GetElapsedTime(started).TotalMilliseconds;
                LogFailed(logger, ex, messageName, elapsedMs);
            }
            throw;
        }
    }

    // Source-generated logging: no boxing or string formatting unless the level is enabled.
    [LoggerMessage(Level = LogLevel.Information, Message = "Handled {MessageName} in {ElapsedMs:0.0} ms")]
    private static partial void LogHandled(ILogger logger, string messageName, double elapsedMs);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{MessageName} failed after {ElapsedMs:0.0} ms")]
    private static partial void LogFailed(ILogger logger, Exception exception, string messageName, double elapsedMs);
}
