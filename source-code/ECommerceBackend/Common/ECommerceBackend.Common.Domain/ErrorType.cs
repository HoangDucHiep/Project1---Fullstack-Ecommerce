namespace ECommerceBackend.Common.Domain;


public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    Problem = 2,
    NotFound = 3,
    Conflict = 4,
    Unauthorized = 5,
    Forbidden = 6,
    BadRequest = 7,
    MethodNotAllowed = 8,
    UnsupportedMediaType = 9,
    UnprocessableEntity = 10,
    RequestTimeout = 11,
    TooManyRequests = 12,
    NotImplemented = 13,
    ServiceUnavailable = 14,
    Gone = 15,
    PreconditionFailed = 16
}
