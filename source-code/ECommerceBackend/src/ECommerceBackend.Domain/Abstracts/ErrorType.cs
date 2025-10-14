namespace ECommerceBackend.Domain.Abstracts;

/// HDHIep - 09/24/2025 - created
/// <summary>
/// Represents an error type with associated HTTP status code, title, and RFC problem type.
/// Provides predefined error types for common scenarios.
/// </summary>
/// HDHiep - 09/30.2025  - refactored to record
public sealed record ErrorType
{
    /// <summary>
    /// Gets the HTTP status code associated with this error type.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the human-readable title for this error type.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the RFC problem type URI for this error type.
    /// </summary>
    public string ProblemType { get; }

    /// <summary>
    /// Gets the unique identifier for this error type.
    /// </summary>
    public string Name { get; }

    private ErrorType(string name, int statusCode, string title, string problemType)
    {
        StatusCode = statusCode;
        Title = title;
        ProblemType = problemType;
        Name = name;
    }

    // -- Predefined Error Types -- //
    /// <summary>
    /// Represents a validation error (HTTP 400).
    /// </summary>
    /// <remarks>
    /// See <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1">RFC 7231 Section 6.5.1</a>.
    /// </remarks>
    public static readonly ErrorType Failure = new(
        nameof(Failure),
        500,
        "Server Failure",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1");

    public static readonly ErrorType Validation = new(
        nameof(Validation),
        400,
        "Validation Error",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1");

    public static readonly ErrorType Problem = new(
        nameof(Problem),
        500,
        "Problem",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1");

    public static readonly ErrorType NotFound = new(
        nameof(NotFound),
        404,
        "Not Found",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4");

    public static readonly ErrorType Conflict = new(
        nameof(Conflict),
        409,
        "Conflict",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8");

    public static readonly ErrorType Unauthorized = new(
        nameof(Unauthorized),
        401,
        "Unauthorized",
        "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1");

    public static readonly ErrorType Forbidden = new(
        nameof(Forbidden),
        403,
        "Forbidden",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3");

    public static readonly ErrorType BadRequest = new(
        nameof(BadRequest),
        400,
        "Bad Request",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1");

    public static readonly ErrorType MethodNotAllowed = new(
        nameof(MethodNotAllowed),
        405,
        "Method Not Allowed",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.5");

    public static readonly ErrorType UnsupportedMediaType = new(
        nameof(UnsupportedMediaType),
        415,
        "Unsupported Media Type",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.13");

    public static readonly ErrorType UnprocessableEntity = new(
        nameof(UnprocessableEntity),
        422,
        "Unprocessable Entity",
        "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2");

    public static readonly ErrorType RequestTimeout = new(
        nameof(RequestTimeout),
        408,
        "Request Timeout",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.7");

    public static readonly ErrorType TooManyRequests = new(
        nameof(TooManyRequests),
        429,
        "Too Many Requests",
        "https://datatracker.ietf.org/doc/html/rfc6585#section-4");

    public static readonly ErrorType NotImplemented = new(
        nameof(NotImplemented),
        501,
        "Not Implemented",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.2");

    public static readonly ErrorType ServiceUnavailable = new(
        nameof(ServiceUnavailable),
        503,
        "Service Unavailable",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4");

    public static readonly ErrorType Gone = new(
        nameof(Gone),
        410,
        "Gone",
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.9");

    public static ErrorType CustomType(int statusCode, string title, string problemType) =>
        new("Custom", statusCode, title, problemType);

    public override string ToString() => Name;

    // implicit conversion
    public static implicit operator string(ErrorType errorType) => errorType.Name;
}
