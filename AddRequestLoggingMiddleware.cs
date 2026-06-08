using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

public class AddRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AddRequestLoggingMiddleware> _logger;

    public AddRequestLoggingMiddleware(RequestDelegate next, ILogger<AddRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Generate a short 8-character correlation ID
        string correlationId = Guid.NewGuid().ToString("N")[..8];

        // 2. Stamp the response header before moving down the pipeline
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        // 3. Start timing the request
        var stopwatch = Stopwatch.StartNew();

        // 4. Log on entry (Method, Path, Correlation ID)
        _logger.LogInformation("HTTP {Method} {Path} started. [Correlation ID: {CorrelationId}]",
            context.Request.Method, context.Request.Path, correlationId);

        try
        {
            // Hand over to the next middleware in the pipeline
            await _next(context);
        }
        finally
        {
            // 5. Stop timing and log on exit (Status Code, Elapsed ms, Correlation ID)
            stopwatch.Stop();
            _logger.LogInformation("HTTP {Method} {Path} finished with Status {StatusCode} in {ElapsedMs}ms. [Correlation ID: {CorrelationId}]",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, correlationId);
        }
    }
}