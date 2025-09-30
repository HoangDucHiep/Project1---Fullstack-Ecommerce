namespace ECommerceBackend.Domain.Abstracts;


/// HDHIep - 09/24/2025
/// <summary>
/// Represents an application error with a code, human-readable description, and a categorized <see cref="ErrorType"/>.
/// Use the static factory methods to create consistent errors across the application.
/// </summary>
public record Error
{
    /// <summary>
    /// A stable programmatic error code (e.g., "Order.InvalidState").
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// A human-readable description intended for logs or client messages.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The error category, typically mapped to HTTP status codes at the API boundary.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Represents the absence of an error.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    /// <summary>
    /// Represents a null value error. Commonly used for guard clauses.
    /// </summary>
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    /// <summary>
    /// Creates a new <see cref="Error"/>.
    /// Prefer using the specialized factory methods (e.g., <see cref="BadRequest(string, string)"/>).
    /// </summary>
    /// <param name="code">A stable programmatic code (e.g., "Order.InvalidState").</param>
    /// <param name="description">Human-readable description.</param>
    /// <param name="type">The <see cref="ErrorType"/> category.</param>
    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// Creates a server-side failure error (HTTP 500).
    /// </summary>
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    /// <summary>
    /// Creates a validation error (typically HTTP 400).
    /// </summary>
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    /// <summary>
    /// Creates a problem error for RFC 7807 problem details scenarios.
    /// </summary>
    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    /// <summary>
    /// Creates a not found error (HTTP 404).
    /// </summary>
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>
    /// Creates a conflict error (HTTP 409).
    /// </summary>
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized error (HTTP 401).
    /// </summary>
    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    /// <summary>
    /// Creates a forbidden error (HTTP 403).
    /// </summary>
    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);

    /// <summary>
    /// Creates a bad request error (HTTP 400).
    /// </summary>
    public static Error BadRequest(string code, string description) =>
        new(code, description, ErrorType.BadRequest);

    /// <summary>
    /// Creates a method not allowed error (HTTP 405).
    /// </summary>
    public static Error MethodNotAllowed(string code, string description) =>
        new(code, description, ErrorType.MethodNotAllowed);

    /// <summary>
    /// Creates an unsupported media type error (HTTP 415).
    /// </summary>
    public static Error UnsupportedMediaType(string code, string description) =>
        new(code, description, ErrorType.UnsupportedMediaType);

    /// <summary>
    /// Creates an unprocessable entity error (HTTP 422).
    /// </summary>
    public static Error UnprocessableEntity(string code, string description) =>
        new(code, description, ErrorType.UnprocessableEntity);

    /// <summary>
    /// Creates a request timeout error (HTTP 408).
    /// </summary>
    public static Error RequestTimeout(string code, string description) =>
        new(code, description, ErrorType.RequestTimeout);

    /// <summary>
    /// Creates a too many requests error (HTTP 429).
    /// </summary>
    public static Error TooManyRequests(string code, string description) =>
        new(code, description, ErrorType.TooManyRequests);

    /// <summary>
    /// Creates a not implemented error (HTTP 501).
    /// </summary>
    public static Error NotImplemented(string code, string description) =>
        new(code, description, ErrorType.NotImplemented);

    /// <summary>
    /// Creates a service unavailable error (HTTP 503).
    /// </summary>
    public static Error ServiceUnavailable(string code, string description) =>
        new(code, description, ErrorType.ServiceUnavailable);

    /// <summary>
    /// Creates a gone error (HTTP 410).
    /// </summary>
    public static Error Gone(string code, string description) =>
        new(code, description, ErrorType.Gone);

    /// <summary>
    /// Creates a precondition failed error (HTTP 412).
    /// </summary>
    //public static Error PreconditionFailed(string code, string description) =>
    //    new(code, description, ErrorType.PreconditionFailed);
}
