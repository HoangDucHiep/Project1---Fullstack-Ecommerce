using ECommerceBackend.Common.Domain;

namespace ECommerceBackend.Modules.Users.Application.Identity;

public static class IdentityProviderErrors
{
    public static readonly Error PhoneIsNotUnique = Error.Conflict(
        code: "Identity.PhoneIsNotUnique",
        description: "The phone number is already registered."
    );
}

