using ECommerceBackend.Domain.Abstracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ApplicationException = ECommerceBackend.Application.Abstracts.Exceptions.ApplicationException;

namespace ECommerceBackend.Api.Middlewares;


/// HDHiep - 09/24/2025
internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        ProblemDetails problemsDetails = exception switch
        {
            ApplicationException appEx when appEx.Error is not null => CreateProblemDetails(appEx.Error),
            ApplicationException appEx => CreateDefaultApplicationProblemDetails(appEx),
            _ => CreateDefaultProblemDetails()
        };

        httpContext.Response.StatusCode = problemsDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(problemsDetails, cancellationToken).ConfigureAwait(false);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(Error error)
    {
        return new ProblemDetails
        {
            Status = error.Type.StatusCode,
            Title = error.Type.Title,
            Type = error.Type.ProblemType,
            Detail = error.Description,
            Extensions = { ["errorCode"] = error.Code }
        };
    }

    private static ProblemDetails CreateDefaultApplicationProblemDetails(ApplicationException appException)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Detail = $"An error occurred while processing request: {appException.RequestName}"
        };
    }

    private static ProblemDetails CreateDefaultProblemDetails()
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure",
            Detail = "An unexpected error occurred while processing the request."
        };
    }
}
