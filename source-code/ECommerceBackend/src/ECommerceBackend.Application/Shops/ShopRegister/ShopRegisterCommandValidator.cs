using FluentValidation;

namespace ECommerceBackend.Application.Shops.ShopRegister;
internal sealed class ShopRegisterCommandValidator : AbstractValidator<ShopRegisterCommand>
{
    public ShopRegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Shop name is required.")
            .MaximumLength(200).WithMessage("Shop name must not exceed 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
        RuleFor(x => x.LogoUrl)
            .MaximumLength(500).WithMessage("Logo URL must not exceed 500 characters.")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrEmpty(x.LogoUrl))
            .WithMessage("Logo URL must be a valid URL.");
        RuleFor(x => x.BannerUrl)
            .MaximumLength(500).WithMessage("Banner URL must not exceed 500 characters.")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrEmpty(x.BannerUrl))
            .WithMessage("Banner URL must be a valid URL.");
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Owner ID is required.");
    }

}
