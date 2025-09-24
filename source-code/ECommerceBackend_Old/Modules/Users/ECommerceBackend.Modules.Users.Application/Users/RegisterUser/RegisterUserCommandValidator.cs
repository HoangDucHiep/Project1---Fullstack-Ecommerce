using FluentValidation;

namespace ECommerceBackend.Modules.Users.Application.Users.RegisterUser;

/// HoangDucHiep - 08/2025
/// <summary>
/// Provides validation rules for the <see cref="RegisterUserCommand"/>.HoangDucHiep - 08/2025
/// </summary>
/// <remarks>This validator ensures that the <see cref="RegisterUserCommand.Phone"/> property is not null and that
/// the <see cref="RegisterUserCommand.Password"/> property is not null and meets the minimum length
/// requirement.</remarks> 
internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Phone).NotNull();
        RuleFor(c => c.Password).NotNull().MinimumLength(6);
    }
}
