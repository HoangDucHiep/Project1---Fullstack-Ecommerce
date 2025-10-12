using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Exceptions;
public class ApplicationInvalidOperationException : ApplicationException
{
    public ApplicationInvalidOperationException(Error error, Exception? innerException = default)
        : base("InvalidOperationException", error, innerException)
    {
    }


}
