using System.Text.Json.Serialization;
using ECommerceBackend.Application.Contracts.Commons;

namespace ECommerceBackend.Api.Contracts;

/// <summary>
/// Standard API response wrapper for success responses
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseMetadata? Metadata { get; set; }

    public string TraceId { get; set; } = string.Empty;

    public static ApiResponse<T> Ok(T data, string message = "Thành công") => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    public static ApiResponse<List<TItem>> Paginated<TItem>(
        PaginationResult<TItem> result,
        string message = "Thành công") => new()
        {
            Success = true,
            Message = message,
            Data = result.Items as dynamic,
            Metadata = new ResponseMetadata
            {
                Pagination = new PaginationMeta
                {
                    CurrentPage = result.Page,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount,
                    TotalPages = result.TotalPages,
                    HasPreviousPage = result.HasPreviousPage,
                    HasNextPage = result.HasNextPage
                }
            }
        };
}

/// <summary>
/// Standard API error response
/// </summary>
public class ApiErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorDetail>? Errors { get; set; }

    public string TraceId { get; set; } = string.Empty;

    public static ApiErrorResponse Error(string errorCode, string message, List<ErrorDetail>? errors = null) => new()
    {
        Success = false,
        Message = message,
        ErrorCode = errorCode,
        Errors = errors
    };
}

/// <summary>
/// Response metadata containing pagination, links, etc.
/// </summary>
public class ResponseMetadata
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationMeta? Pagination { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<LinkDto>? Links { get; set; }
}

/// <summary>
/// Pagination metadata
/// </summary>
public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

/// <summary>
/// Error detail with optional inner errors
/// </summary>
public class ErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorDetail>? InnerErrors { get; set; }
}

/// <summary>
/// HATEOAS link (for future use)
/// </summary>
public class LinkDto
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
}

