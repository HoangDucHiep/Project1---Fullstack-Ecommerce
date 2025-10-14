using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Exceptions;
public class ApplicationArgumentException : ApplicationException
{
    public ApplicationArgumentException(Error error, Exception? innerException = default)
        : base("ArgumentException", error, innerException)
    {
    }
}
