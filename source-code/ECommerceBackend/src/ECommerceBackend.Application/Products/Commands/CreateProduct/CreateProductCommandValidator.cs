using ECommerceBackend.Application.Contracts.Products;
using FluentValidation;

namespace ECommerceBackend.Application.Products.Commands.CreateProduct;

/// <summary>
/// Validator for CreateProductCommand
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ShopId)
            .NotEmpty()
            .WithMessage("Shop ID is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MinimumLength(5)
            .WithMessage("Product name must be at least 5 characters")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Product description is required")
            .MaximumLength(1000)
            .WithMessage("Product description cannot exceed 1000 characters");

        RuleFor(x => x.Media)
            .NotNull()
            .WithMessage("Product must have at least one image")
            .Must(media => media != null && media.Count > 0)
            .WithMessage("Product must have at least one image")
            .Must(media => media == null || media.Count <= 10)
            .WithMessage("Product cannot have more than 10 media files (9 images + 1 video)");

        RuleFor(x => x.Media)
            .Must(media => media == null || media.Count(m => m.MediaType.Equals("Video", StringComparison.OrdinalIgnoreCase)) <= 1)
            .WithMessage("Product cannot have more than 1 video");

        RuleFor(x => x.Media)
            .Must(media => media == null || media.Count(m => m.IsCover) <= 1)
            .WithMessage("Product cannot have more than 1 cover image");

        RuleFor(x => x.Media)
            .Must(media => media == null || media.Any(m => m.IsCover))
            .WithMessage("Product must have at least one cover image");

        RuleFor(x => x.Options)
            .NotNull()
            .WithMessage("Product options are required")
            .Must(options => options == null || options.Count <= 2)
            .WithMessage("Product cannot have more than 2 option types");

        RuleForEach(x => x.Options)
            .ChildRules(option =>
            {
                option.RuleFor(o => o.Name)
                    .NotEmpty()
                    .WithMessage("Option name is required");

                option.RuleFor(o => o.Values)
                    .NotNull()
                    .WithMessage("Option values are required")
                    .Must(values => values != null && values.Count > 0)
                    .WithMessage("Option must have at least one value");
            });

        RuleFor(x => x.Options)
            .Must(options => options == null || options.Select(o => o.Name).Distinct().Count() == options.Count)
            .WithMessage("Option names must be unique");

        RuleFor(x => x.Variants)
            .NotNull()
            .WithMessage("Product must have at least one variant")
            .Must(variants => variants != null && variants.Count > 0)
            .WithMessage("Product must have at least one variant");

        RuleForEach(x => x.Variants)
            .ChildRules(variant =>
            {
                variant.RuleFor(v => v.Sku)
                    .NotEmpty()
                    .WithMessage("SKU is required")
                    .MinimumLength(3)
                    .WithMessage("SKU must be at least 3 characters")
                    .MaximumLength(100)
                    .WithMessage("SKU cannot exceed 100 characters");

                variant.RuleFor(v => v.Price)
                    .GreaterThan(0)
                    .WithMessage("Price must be greater than 0");

                variant.RuleFor(v => v.Stock)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Stock must be greater than or equal to 0");

                variant.RuleFor(v => v.Weight)
                    .GreaterThan(0)
                    .WithMessage("Weight must be greater than 0");

                variant.RuleFor(v => v.Height)
                    .GreaterThan(0)
                    .WithMessage("Height must be greater than 0");

                variant.RuleFor(v => v.Width)
                    .GreaterThan(0)
                    .WithMessage("Width must be greater than 0");

                variant.RuleFor(v => v.Length)
                    .GreaterThan(0)
                    .WithMessage("Length must be greater than 0");

                variant.RuleFor(v => v.VariantMedia)
                    .Must(media => media == null || media.Count(m => m.IsCover) <= 1)
                    .WithMessage("Variant cannot have more than 1 cover image");
            });

        RuleFor(x => x.Variants)
            .Must(variants => variants == null || variants.Select(v => v.Sku).Distinct().Count() == variants.Count)
            .WithMessage("SKUs must be unique");
    }
}

