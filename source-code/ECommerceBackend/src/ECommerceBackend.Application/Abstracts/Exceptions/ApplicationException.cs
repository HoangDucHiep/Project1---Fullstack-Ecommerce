using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Exceptions;

/// HDHiep - 09/24/2025
/// <summary>
/// Exception type for application-layer failures that includes the request name and optional domain <see cref="Error"/>.
/// </summary>
public class ApplicationException : Exception
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationException"/> class.
    /// </summary>
    /// <param name="requestName">The name of the request or handler where the exception occurred.</param>
    /// <param name="error">Optional domain error associated with the failure.</param>
    /// <param name="innerException">Optional inner exception.</param>
    public ApplicationException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }


    /// <summary>
    /// Gets the request/operation name associated with this exception.
    /// </summary>
    public string RequestName { get; }

    /// <summary>
    /// Gets the associated domain error, if any.
    /// </summary>
    public Error? Error { get; }
}
