using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;
public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("Users.NotFound", $"The user with the identifier {userId} not found");

    public static Error NotFound(string identityId) =>
        Error.NotFound("Users.NotFound", $"The user with the IDP identifier {identityId} not found");

    public static Error InvalidCredentials => Error.Validation("Users.InvalidCredentials", "The provided credentials are invalid");
}
