using FluentValidation;

namespace ECommerceBackend.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID không được để trống");

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

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU không được để trống")
            .MaximumLength(100)
            .WithMessage("SKU không được vượt quá 100 ký tự");

        // Validate Images
        RuleFor(x => x.Images)
            .NotNull()
            .WithMessage("Danh sách ảnh không được null")
            .Must(x => x.Count > 0)
            .WithMessage("Sản phẩm phải có ít nhất 1 hình ảnh")
            .Must(x => x.Count <= 10)
            .WithMessage("Sản phẩm không được có quá 10 hình ảnh")
            .Must(HaveExactlyOneCoverImage)
            .WithMessage("Sản phẩm phải có đúng 1 ảnh cover")
            .Must(CoverImageHasSortOrderZero)
            .WithMessage("Ảnh cover phải có sortOrder = 0")
            .Must(HaveUniqueImageSortOrders)
            .WithMessage("Thứ tự sắp xếp ảnh không được trùng lặp");

        RuleForEach(x => x.Images)
            .SetValidator(new UpdateProductImageDtoValidator());

        // Validate Video (optional)
        RuleFor(x => x.Video)
            .SetValidator(new UpdateProductVideoDtoValidator()!)
            .When(x => x.Video != null);

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
            .SetValidator(new UpdateProductOptionDtoValidator())
            .When(x => x.Options.Any());

        // Validate variants when present
        RuleForEach(x => x.Variants)
            .SetValidator(new UpdateProductVariantDtoValidator())
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

    private static bool HaveValidProductType(UpdateProductCommand command)
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

    private static bool HaveValidVariantCombinations(UpdateProductCommand command)
    {
        if (!command.Options.Any() || !command.Variants.Any())
        {
            return true;
        }

        var allOptionValues = command.Options
            .SelectMany(o => o.Values)
            .Select(v => v.Value)
            .ToList();

        foreach (UpdateProductVariantDto variant in command.Variants)
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

    private static bool HaveExactlyOneCoverImage(List<UpdateProductImageDto> images)
    {
        return images.Count(m => m.IsCover) == 1;
    }

    private static bool CoverImageHasSortOrderZero(List<UpdateProductImageDto> images)
    {
        UpdateProductImageDto? coverImage = images.FirstOrDefault(m => m.IsCover);
        return coverImage != null && coverImage.SortOrder == 0;
    }

    private static bool HaveUniqueImageSortOrders(List<UpdateProductImageDto> images)
    {
        var sortOrders = images.Select(m => m.SortOrder).ToList();
        return sortOrders.Count == sortOrders.Distinct().Count();
    }
}

public class UpdateProductImageDtoValidator : AbstractValidator<UpdateProductImageDto>
{
    public UpdateProductImageDtoValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage("URL ảnh là bắt buộc")
            .MaximumLength(500)
            .WithMessage("URL ảnh không được vượt quá 500 ký tự");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Thứ tự sắp xếp phải >= 0");
    }
}

public class UpdateProductVideoDtoValidator : AbstractValidator<UpdateProductVideoDto>
{
    public UpdateProductVideoDtoValidator()
    {
        RuleFor(x => x.VideoUrl)
            .NotEmpty()
            .WithMessage("URL video là bắt buộc")
            .MaximumLength(500)
            .WithMessage("URL video không được vượt quá 500 ký tự");
    }
}

public class UpdateProductOptionDtoValidator : AbstractValidator<UpdateProductOptionDto>
{
    public UpdateProductOptionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên option không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên option không được vượt quá 100 ký tự");

        RuleFor(x => x.Values)
            .NotNull()
            .WithMessage("Danh sách giá trị option không được null")
            .Must(x => x.Count > 0)
            .WithMessage("Option phải có ít nhất 1 giá trị");

        RuleForEach(x => x.Values)
            .SetValidator(new UpdateProductOptionValueDtoValidator());
    }
}

public class UpdateProductOptionValueDtoValidator : AbstractValidator<UpdateProductOptionValueDto>
{
    public UpdateProductOptionValueDtoValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Giá trị option không được để trống")
            .MaximumLength(100)
            .WithMessage("Giá trị option không được vượt quá 100 ký tự");
    }
}

public class UpdateProductVariantDtoValidator : AbstractValidator<UpdateProductVariantDto>
{
    public UpdateProductVariantDtoValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Giá variant phải lớn hơn 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tồn kho variant không được âm");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU variant không được để trống")
            .MaximumLength(100)
            .WithMessage("SKU variant không được vượt quá 100 ký tự");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Weight.HasValue)
            .WithMessage("Trọng lượng không được âm");

        RuleFor(x => x.Height)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Height.HasValue)
            .WithMessage("Chiều cao không được âm");

        RuleFor(x => x.Width)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Width.HasValue)
            .WithMessage("Chiều rộng không được âm");

        RuleFor(x => x.Length)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Length.HasValue)
            .WithMessage("Chiều dài không được âm");

        // Validate variant images
        RuleFor(x => x.Images)
            .Must(x => x == null || x.Count <= 3)
            .WithMessage("Variant không được có quá 3 hình ảnh")
            .Must(x => x == null || x.Count(m => m.IsCover) <= 1)
            .WithMessage("Variant chỉ được có tối đa 1 ảnh cover")
            .Must(x => x == null || VariantCoverImageHasSortOrderZero(x))
            .WithMessage("Ảnh cover của variant phải có sortOrder = 0")
            .Must(x => x == null || HaveUniqueVariantImageSortOrders(x))
            .WithMessage("Thứ tự sắp xếp ảnh trong variant không được trùng lặp");

        RuleForEach(x => x.Images)
            .SetValidator(new UpdateProductImageDtoValidator())
            .When(x => x.Images != null);
    }

    private static bool VariantCoverImageHasSortOrderZero(List<UpdateProductImageDto> images)
    {
        UpdateProductImageDto? coverImage = images.FirstOrDefault(m => m.IsCover);
        return coverImage == null || coverImage.SortOrder == 0;
    }

    private static bool HaveUniqueVariantImageSortOrders(List<UpdateProductImageDto> images)
    {
        var sortOrders = images.Select(m => m.SortOrder).ToList();
        return sortOrders.Count == sortOrders.Distinct().Count();
    }
}
