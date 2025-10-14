using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;
public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("Users.NotFound", $"The user with the identifier {userId} not found");

    public static Error NotFound(string identityId) =>
        Error.NotFound("Users.NotFound", $"The user with the IDP identifier {identityId} not found");

    public static Error NotFound() =>
        Error.NotFound("Users.NotFound", $"The user not found");

    public static Error InvalidCredentials => Error.Validation("Users.InvalidCredentials", "The provided credentials are invalid");

    public static Error PhoneNumberAlreadyRegistered(string phoneNumber) =>
        Error.BadRequest("Users.PhoneNumberAlreadyRegistered", $"The phone number {phoneNumber} is already registered");

    public static Error EmailAlreadyExists(string email) =>
        Error.Conflict("Users.EmailAlreadyExists", $"The email '{email}' is already registered");

    public static Error PhoneAlreadyExists(string phone) =>
        Error.Conflict("Users.PhoneAlreadyExists", $"The phone number '{phone}' is already registered");

}
