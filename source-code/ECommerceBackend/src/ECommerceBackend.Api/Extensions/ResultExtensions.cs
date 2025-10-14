using ECommerceBackend.Api.Contracts;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Api.Extensions;

/// <summary>
/// Extension methods to convert Result to ApiResponse
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Convert Result to ApiResponse with custom success message
    /// </summary>
    public static object ToResponse<T>(this Result<T> result, string successMessage = "Thành công")
    {
        if (result.IsSuccess)
        {
            return ApiResponse<T>.Ok(result.Value, successMessage);
        }

        return CreateErrorResponse(result.Error);
    }

    /// <summary>
    /// Convert Result (non-generic) to ApiResponse
    /// </summary>
    public static object ToResponse(this Result result, string successMessage = "Thành công")
    {
        if (result.IsSuccess)
        {
            return ApiResponse<object?>.Ok(null, successMessage);
        }

        return CreateErrorResponse(result.Error);
    }

    /// <summary>
    /// Convert PaginationResult to ApiResponse with pagination metadata
    /// </summary>
    public static object ToPaginatedResponse<T>(
        this Result<Application.Contracts.Commons.PaginationResult<T>> result,
        string successMessage = "Thành công")
    {
        if (result.IsSuccess)
        {
            return ApiResponse<List<T>>.Paginated(result.Value, successMessage);
        }

        return CreateErrorResponse(result.Error);
    }

    /// <summary>
    /// Get HTTP status code from Error
    /// </summary>
    public static int GetStatusCode(this Error error)
    {
        return error.Type.StatusCode;
    }

    /// <summary>
    /// Create error response from Error, handling ValidationError specially
    /// </summary>
    private static ApiErrorResponse CreateErrorResponse(Error error)
    {
        if (error is ValidationError validationError)
        {
            var errorDetails = validationError.Errors
                .Select(e => new ErrorDetail
                {
                    Code = e.Code,
                    Message = e.Description,
                    Field = ExtractFieldFromErrorCode(e.Code)
                })
                .ToList();

            return ApiErrorResponse.Error(
                validationError.Code,
                validationError.Description,
                errorDetails
            );
        }

        return ApiErrorResponse.Error(
            error.Code,
            error.Description
        );
    }

    private static string? ExtractFieldFromErrorCode(string errorCode)
    {
        // Extract field name from error code like "Name.Invalid" -> "name"
        string[] parts = errorCode.Split('.');
        if (parts.Length >= 1)
        {
            // Get the first part (field name) and convert to camelCase
            string fieldName = parts[0];
            if (fieldName.Length > 0)
            {
                return char.ToLowerInvariant(fieldName[0]) + (fieldName.Length > 1 ? fieldName[1..] : "");
            }
        }
        return null;
    }
}

