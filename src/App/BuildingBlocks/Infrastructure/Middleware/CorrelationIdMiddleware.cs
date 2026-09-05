using App.BuildingBlocks.Infrastructure.Observability;
using Serilog.Context;

namespace App.BuildingBlocks.Infrastructure.Middleware;

/// <summary>
/// Assigns (or propagates) a correlation id for every request and pushes it onto
/// the log context so every structured log line for that request carries it.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing) && existing.Count > 0
            ? existing.ToString()
            : Guid.NewGuid().ToString();

        context.Items[HttpContextCorrelationIdProvider.HttpContextItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
