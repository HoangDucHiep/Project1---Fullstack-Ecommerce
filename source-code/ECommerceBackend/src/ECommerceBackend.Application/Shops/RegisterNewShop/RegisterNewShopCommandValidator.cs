using FluentValidation;

namespace ECommerceBackend.Application.Shops.RegisterNewShop;

internal sealed class RegisterNewShopCommandValidator : AbstractValidator<RegisterNewShopCommand>
{
    public RegisterNewShopCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên shop là bắt buộc")
            .MaximumLength(200)
            .WithMessage("Tên shop không được vượt quá 200 ký tự");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Mô tả shop là bắt buộc")
            .MaximumLength(2000)
            .WithMessage("Mô tả không được vượt quá 2000 ký tự");

        RuleFor(x => x.LogoUrl)
            .NotEmpty()
            .WithMessage("Logo URL là bắt buộc")
            .MaximumLength(500)
            .WithMessage("Logo URL không được vượt quá 500 ký tự");

        RuleFor(x => x.BannerUrl)
            .NotEmpty()
            .WithMessage("Banner URL là bắt buộc")
            .MaximumLength(500)
            .WithMessage("Banner URL không được vượt quá 500 ký tự");

        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Owner ID là bắt buộc");
    }
}
