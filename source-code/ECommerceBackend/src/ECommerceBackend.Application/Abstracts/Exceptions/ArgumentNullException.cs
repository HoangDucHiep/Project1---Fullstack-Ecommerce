using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Exceptions;

/// HDHiep - 10/07/2025
/// <summary>
/// Exception type for null argument errors that includes an <see cref="Error"/>.
/// </summary>
public sealed class ArgumentNullException : ApplicationException
{
    public ArgumentNullException(Error error, Exception? innerException = default)
        : base("ArgumentNullException", error, innerException)
    {
    }
}
