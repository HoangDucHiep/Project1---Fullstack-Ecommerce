using ECommerceBackend.Api.Contracts;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.AspNetCore.Diagnostics;
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

        (int statusCode, ApiErrorResponse response) = exception switch
        {
            ApplicationException appEx when appEx.Error is ValidationError validationError =>
                (validationError.Type.StatusCode, CreateValidationErrorResponse(validationError)),
            ApplicationException appEx when appEx.Error is not null =>
                (appEx.Error.Type.StatusCode, ApiErrorResponse.Error(appEx.Error.Code, appEx.Error.Description)),
            ApplicationException appEx =>
                (StatusCodes.Status400BadRequest, ApiErrorResponse.Error("Application.Error", $"Lỗi xử lý yêu cầu: {appEx.RequestName}")),
            _ =>
                (StatusCodes.Status500InternalServerError, ApiErrorResponse.Error("Server.InternalError", "Đã xảy ra lỗi hệ thống"))
        };

        response.TraceId = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken).ConfigureAwait(false);

        return true;
    }

    private static ApiErrorResponse CreateValidationErrorResponse(ValidationError validationError)
    {
        var errorDetails = validationError.Errors
            .Select(e => new ErrorDetail
            {
                Code = e.Code,
                Message = e.Description,
                Field = ExtractFieldFromErrorCode(e.Code)
            })
            .ToList();

        // Always return error details, even if empty
        var response = ApiErrorResponse.Error(
            validationError.Code,
            validationError.Description,
            errorDetails.Count > 0 ? errorDetails : null
        );

        return response;
    }

    private static string? ExtractFieldFromErrorCode(string errorCode)
    {
        // Extract field name from error code like "Name.Invalid" -> "name"
        // or "Email.Required" -> "email"
        string[] parts = errorCode.Split('.');
        if (parts.Length >= 1)
        {
            // Get the first part (field name) and convert to camelCase
            string fieldName = parts[0];
            return char.ToLowerInvariant(fieldName[0]) + fieldName[1..];
        }
        return null;
    }
}
