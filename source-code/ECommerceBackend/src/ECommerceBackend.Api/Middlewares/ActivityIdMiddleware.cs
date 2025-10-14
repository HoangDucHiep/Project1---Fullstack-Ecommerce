using System.Diagnostics;

namespace ECommerceBackend.Api.Middlewares;

/// <summary>
/// Middleware that logs Activity.Current.Id for debugging
/// </summary>
public class ActivityIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityIdMiddleware> _logger;

    public ActivityIdMiddleware(RequestDelegate next, ILogger<ActivityIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? activityId = Activity.Current?.Id;
        string traceIdentifier = context.TraceIdentifier;

        _logger.LogInformation(
            "Activity.Current.Id: {ActivityId}, HttpContext.TraceIdentifier: {TraceIdentifier}",
            activityId ?? "null",
            traceIdentifier
        );

        await _next(context);
    }
}

