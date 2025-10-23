namespace ECommerceBackend.Application.Abstracts.Authentication;

/// HDHiep - 10/07/2025
/// <summary>
/// User context interface to access current user's information
/// </summary>
/// <remarks>
/// - IdentityUserId: Current user's identity ID from JWT claims
/// - UserId: Current user's unique identifier
/// - Email: Current user's email address
/// - PhoneNumber: Current user's phone number
/// - IsAuthenticated: Indicates if the user is authenticated
/// </remarks>
public interface IUserContext
{
    string? IdentityUserId { get; }
    string? UserId { get; }
    string? Email { get; }
    string? PhoneNumber { get; }
    bool IsAuthenticated { get; }
}
