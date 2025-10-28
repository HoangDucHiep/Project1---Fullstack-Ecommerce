using FluentValidation;

namespace ECommerceBackend.Application.Products.Commands.CreateNewProduct;

public class CreateNewProductCommandValidator : AbstractValidator<CreateNewProductCommand>
{
    public CreateNewProductCommandValidator()
    {
        RuleFor(x => x.ShopId)
            .NotEmpty()
            .WithMessage("Shop ID không được để trống");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID không được để trống");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(200)
            .WithMessage("Tên sản phẩm không được vượt quá 200 ký tự");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Mô tả sản phẩm không được để trống")
            .MaximumLength(2000)
            .WithMessage("Mô tả sản phẩm không được vượt quá 2000 ký tự");

        RuleFor(x => x.Medias)
            .NotNull()
            .WithMessage("Danh sách media không được null")
            .Must(x => x.Count > 0)
            .WithMessage("Sản phẩm phải có ít nhất 1 hình ảnh")
            .Must(x => x.Count <= 10)
            .WithMessage("Sản phẩm không được có quá 10 hình ảnh")
            .Must(HaveOnlyOneCoverImage)
            .WithMessage("Sản phẩm chỉ được có 1 ảnh cover")
            .Must(HaveUniqueSortOrders)
            .WithMessage("Thứ tự sắp xếp media không được trùng lặp");

        RuleForEach(x => x.Medias)
            .SetValidator(new CreateProductMediaDtoValidator());

        RuleFor(x => x.Options)
            .NotNull()
            .WithMessage("Danh sách options không được null");

        RuleFor(x => x.Variants)
            .NotNull()
            .WithMessage("Danh sách variants không được null");

        // Main validation: Options vs Variants vs Simple Product
        RuleFor(x => x)
            .Must(HaveValidProductType)
            .WithMessage("Loại sản phẩm không hợp lệ: Nếu có options thì phải có variants, nếu không có options thì phải cung cấp thông tin sản phẩm đơn giản");

        // Validate options when present
        RuleForEach(x => x.Options)
            .SetValidator(new CreateProductOptionDtoValidator())
            .When(x => x.Options.Any());

        // Validate variants when present
        RuleForEach(x => x.Variants)
            .SetValidator(new CreateProductVariantDtoValidator())
            .When(x => x.Variants.Any());

        // Validate variant combinations when both options and variants are present
        RuleFor(x => x)
            .Must(HaveValidVariantCombinations)
            .When(x => x.Options.Any() && x.Variants.Any())
            .WithMessage("Các variant có tổ hợp option values không hợp lệ");

        // Validate simple product fields when no options/variants
        RuleFor(x => x.DefaultPrice)
            .NotNull()
            .GreaterThan(0)
            .When(x => !x.Options.Any())
            .WithMessage("Giá sản phẩm phải lớn hơn 0 cho sản phẩm đơn giản");

        RuleFor(x => x.DefaultStock)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .When(x => !x.Options.Any())
            .WithMessage("Tồn kho không được âm cho sản phẩm đơn giản");
    }

    private static bool HaveValidProductType(CreateNewProductCommand command)
    {
        bool hasOptions = command.Options.Any();
        bool hasVariants = command.Variants.Any();
        bool hasSimpleFields = command.DefaultPrice.HasValue && command.DefaultStock.HasValue;

        if (hasOptions)
        {
            // Complex product: must have variants, must not have simple fields
            return hasVariants && !hasSimpleFields;
        }
        else
        {
            // Simple product: must have simple fields, must not have variants
            return hasSimpleFields && !hasVariants;
        }
    }

    private static bool HaveValidVariantCombinations(CreateNewProductCommand command)
    {
        if (!command.Options.Any() || !command.Variants.Any())
        {
            return true; // Skip validation if no options or variants
        }

        var allOptionValues = command.Options.SelectMany(o => o.Values).ToList();

        foreach (CreateProductVariantDto variant in command.Variants)
        {
            // Check option values count matches options count
            if (variant.OptionValues.Count != command.Options.Count)
            {
                return false;
            }

            // Check all option values exist in defined options
            if (!variant.OptionValues.All(ov => allOptionValues.Contains(ov)))
            {
                return false;
            }
        }

        // Check no duplicate variant combinations
        var variantCombinations = command.Variants
            .Select(v => string.Join("|", v.OptionValues.OrderBy(x => x)))
            .ToList();
        return variantCombinations.Count == variantCombinations.Distinct().Count();
    }

    private static bool HaveOnlyOneCoverImage(List<CreateProductMediaDto> medias)
    {
        return medias.Count(m => m.IsCover) <= 1;
    }

    private static bool HaveUniqueSortOrders(List<CreateProductMediaDto> medias)
    {
        var sortOrders = medias.Select(m => m.SortOrder).ToList();
        return sortOrders.Count == sortOrders.Distinct().Count();
    }
}

public class CreateProductMediaDtoValidator : AbstractValidator<CreateProductMediaDto>
{
    public CreateProductMediaDtoValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.MediaName) || !string.IsNullOrWhiteSpace(x.MediaUrl))
            .WithMessage("Phải cung cấp ít nhất MediaName hoặc MediaUrl");

        RuleFor(x => x.MediaName)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.MediaName))
            .WithMessage("Tên media không được vượt quá 255 ký tự");

        RuleFor(x => x.MediaUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.MediaUrl))
            .WithMessage("URL media không được vượt quá 500 ký tự");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Thứ tự sắp xếp phải >= 0");
    }
}

public class CreateProductOptionDtoValidator : AbstractValidator<CreateProductOptionDto>
{
    public CreateProductOptionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên option không được để trống")
            .MaximumLength(50)
            .WithMessage("Tên option không được vượt quá 50 ký tự");

        RuleFor(x => x.Values)
            .NotNull()
            .WithMessage("Danh sách values không được null")
            .Must(x => x.Count > 0)
            .WithMessage("Option phải có ít nhất 1 value")
            .Must(x => x.Count <= 20)
            .WithMessage("Option không được có quá 20 values");

        RuleForEach(x => x.Values)
            .NotEmpty()
            .WithMessage("Option value không được để trống")
            .MaximumLength(50)
            .WithMessage("Option value không được vượt quá 50 ký tự");
    }
}

public class CreateProductVariantDtoValidator : AbstractValidator<CreateProductVariantDto>
{
    public CreateProductVariantDtoValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Giá phải lớn hơn 0")
            .LessThanOrEqualTo(999999999)
            .WithMessage("Giá không được vượt quá 999,999,999");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Số lượng tồn kho không được âm")
            .LessThanOrEqualTo(999999)
            .WithMessage("Số lượng tồn kho không được vượt quá 999,999");

        RuleFor(x => x.Sku)
            .MaximumLength(100)
            .WithMessage("SKU không được vượt quá 100 ký tự");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .When(x => x.Weight.HasValue)
            .WithMessage("Trọng lượng phải lớn hơn 0");

        RuleFor(x => x.Height)
            .GreaterThan(0)
            .When(x => x.Height.HasValue)
            .WithMessage("Chiều cao phải lớn hơn 0");

        RuleFor(x => x.Width)
            .GreaterThan(0)
            .When(x => x.Width.HasValue)
            .WithMessage("Chiều rộng phải lớn hơn 0");

        RuleFor(x => x.Length)
            .GreaterThan(0)
            .When(x => x.Length.HasValue)
            .WithMessage("Chiều dài phải lớn hơn 0");

        RuleFor(x => x.Medias)
            .Must(x => x == null || x.Count <= 5)
            .WithMessage("Variant không được có quá 5 hình ảnh")
            .Must(x => x == null || x.Count(m => m.IsCover) <= 1)
            .WithMessage("Variant chỉ được có 1 ảnh cover")
            .Must(x => x == null || HaveUniqueVariantSortOrders(x))
            .WithMessage("Thứ tự sắp xếp media trong variant không được trùng lặp");

        RuleForEach(x => x.Medias)
            .SetValidator(new CreateProductMediaDtoValidator())
            .When(x => x.Medias != null);

        RuleFor(x => x.OptionValues)
            .NotNull()
            .WithMessage("Danh sách option values không được null");
    }

    private static bool HaveUniqueVariantSortOrders(List<CreateProductMediaDto> medias)
    {
        var sortOrders = medias.Select(m => m.SortOrder).ToList();
        return sortOrders.Count == sortOrders.Distinct().Count();
    }
}
