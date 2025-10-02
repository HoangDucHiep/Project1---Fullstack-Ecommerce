namespace ECommerceBackend.Application.Abstracts.Authentication;

public interface IUserContext
{
    string? UserId { get; }
    string? Email { get; }
    string? PhoneNumber { get; }
    bool IsAuthenticated { get; }
}
