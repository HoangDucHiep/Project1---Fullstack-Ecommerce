using FluentValidation;

namespace ECommerceBackend.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID là bắt buộc");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("CategoryId là bắt buộc");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên sản phẩm là bắt buộc")
            .MinimumLength(5)
            .WithMessage("Tên sản phẩm phải có ít nhất 5 ký tự")
            .MaximumLength(200)
            .WithMessage("Tên sản phẩm không được quá 200 ký tự");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Mô tả sản phẩm là bắt buộc")
            .MaximumLength(1000)
            .WithMessage("Mô tả không được quá 1000 ký tự");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU là bắt buộc")
            .MinimumLength(3)
            .WithMessage("SKU phải có ít nhất 3 ký tự")
            .MaximumLength(100)
            .WithMessage("SKU không được quá 100 ký tự");

        RuleFor(x => x.Medias)
            .NotEmpty()
            .WithMessage("Sản phẩm phải có ít nhất 1 media");

        RuleFor(x => x.Options)
            .NotNull()
            .WithMessage("Options không được null");

        RuleFor(x => x.Variants)
            .NotEmpty()
            .WithMessage("Sản phẩm phải có ít nhất 1 variant");
    }
}



