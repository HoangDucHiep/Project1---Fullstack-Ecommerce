using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Exceptions;
public sealed class ConcurrencyException : ApplicationException
{
    public ConcurrencyException(Error error, Exception? innerException = default)
        : base("ConcurrencyException", error, innerException)
    {
    }
}
