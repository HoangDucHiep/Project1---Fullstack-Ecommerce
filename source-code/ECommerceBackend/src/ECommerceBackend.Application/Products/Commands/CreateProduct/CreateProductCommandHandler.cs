#pragma warning disable IDE0008 // Use explicit type
using ECommerceBackend.Application.Abstracts.FileStorage;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Commons;
using ECommerceBackend.Domain.Products;
using ECommerceBackend.Domain.Shops;
using Slugify;

namespace ECommerceBackend.Application.Products.Commands.CreateProduct;

/// <summary>
/// Handler for CreateProductCommand
/// Creates a product with variants, options, and media
/// </summary>
public sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductOptionTypeRepository _productOptionTypeRepository;
    private readonly IProductOptionValueRepository _productOptionValueRepository;
    private readonly IProductVariantOptionValueRepository _productVariantOptionValueRepository;
    private readonly IProductMediaRepository _productMediaRepository;
    private readonly IShopRepository _shopRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SlugHelper _slugHelper;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IProductVariantRepository productVariantRepository,
        IProductOptionTypeRepository productOptionTypeRepository,
        IProductOptionValueRepository productOptionValueRepository,
        IProductVariantOptionValueRepository productVariantOptionValueRepository,
        IProductMediaRepository productMediaRepository,
        IShopRepository shopRepository,
        ICategoryRepository categoryRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productVariantRepository = productVariantRepository;
        _productOptionTypeRepository = productOptionTypeRepository;
        _productOptionValueRepository = productOptionValueRepository;
        _productVariantOptionValueRepository = productVariantOptionValueRepository;
        _productMediaRepository = productMediaRepository;
        _shopRepository = shopRepository;
        _categoryRepository = categoryRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _slugHelper = new SlugHelper();
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // 1. Validate shop exists
        Shop? shop = await _shopRepository.GetByIdAsync(command.ShopId, cancellationToken);
        if (shop is null)
        {
            return Result.Failure<Guid>(ProductErrors.ShopNotFound(command.ShopId));
        }

        // 2. Validate category exists
        Category? category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result.Failure<Guid>(ProductErrors.CategoryNotFound(command.CategoryId));
        }

        // 3. Generate slug and ensure it's unique
        string slug = _slugHelper.GenerateSlug(command.Name);
        Product? existingProduct = await _productRepository.GetBySlugAsync(slug, cancellationToken);
        if (existingProduct is not null)
        {
            // Append a unique identifier to make it unique
            slug = $"{slug}-{Guid.NewGuid().ToString("N")[..8]}";
        }

        // 4. Validate all media files exist
        foreach (var media in command.Media)
        {
            bool mediaExists = await _fileStorageService.FileExistsAsync(media.MediaUrl);
            if (!mediaExists)
            {
                return Result.Failure<Guid>(ProductErrors.MediaNotFound(media.MediaUrl));
            }
        }

        // 5. Validate SKUs are unique in the system
        foreach (var variant in command.Variants)
        {
            ProductVariant? existingVariant = await _productVariantRepository.GetBySkuAsync(variant.Sku, cancellationToken);
            if (existingVariant is not null)
            {
                return Result.Failure<Guid>(ProductErrors.DuplicateSku(variant.Sku));
            }
        }

        // 6. Validate variant option values are valid
        if (command.Options.Count > 0)
        {
            // Validate each variant's option values exist in the defined options
            foreach (var variant in command.Variants)
            {
                if (variant.OptionValues.Count != command.Options.Count)
                {
                    return Result.Failure<Guid>(ProductErrors.InvalidOptionValue(
                        string.Join(", ", variant.OptionValues), 
                        "Number of option values must match number of options"));
                }

                for (int i = 0; i < variant.OptionValues.Count; i++)
                {
                    string optionValue = variant.OptionValues[i];
                    string optionName = command.Options[i].Name;
                    
                    if (!command.Options[i].Values.Contains(optionValue))
                    {
                        return Result.Failure<Guid>(ProductErrors.InvalidOptionValue(optionValue, optionName));
                    }
                }
            }

            // Validate no duplicate variant combinations
            var variantCombinations = command.Variants
                .Select(v => string.Join("|", v.OptionValues))
                .ToList();
            
            if (variantCombinations.Count != variantCombinations.Distinct().Count())
            {
                return Result.Failure<Guid>(ProductErrors.InvalidOptionValue("", "Duplicate variant combinations are not allowed"));
            }
        }

        // 7. Create Product entity
        var product = Product.Create(
            shopId: command.ShopId,
            categoryId: command.CategoryId,
            name: command.Name,
            description: command.Description,
            slug: slug,
            status: ProductStatus.Active
        );

        _productRepository.Add(product);

        // 8. Create ProductMedia entities
        foreach (var mediaDto in command.Media)
        {
            var mediaType = mediaDto.MediaType.Equals("Video", StringComparison.OrdinalIgnoreCase)
                ? MediaType.Video
                : MediaType.Image;

            var productMedia = ProductMedia.CreateProductMedia(
                productId: product.Id,
                mediaUrl: mediaDto.MediaUrl,
                mediaType: mediaType,
                sortOrder: mediaDto.SortOrder,
                isCover: mediaDto.IsCover
            );

            _productMediaRepository.Add(productMedia);
        }

        // 9. Create ProductOptionType and ProductOptionValue entities
        var optionTypeMap = new Dictionary<string, ProductOptionType>(); // optionName -> ProductOptionType
        var optionValueMap = new Dictionary<string, Dictionary<string, ProductOptionValue>>(); // optionName -> (value -> ProductOptionValue)

        foreach (var optionDto in command.Options)
        {
            // Create ProductOptionType
            var optionType = ProductOptionType.Create(
                productId: product.Id,
                name: optionDto.Name
            );

            _productOptionTypeRepository.Add(optionType);
            optionTypeMap[optionDto.Name] = optionType;

            // Create ProductOptionValue for each value
            var valueMap = new Dictionary<string, ProductOptionValue>();
            foreach (var value in optionDto.Values)
            {
                var optionValue = ProductOptionValue.Create(
                    productOptionTypeId: optionType.Id,
                    value: value
                );

                _productOptionValueRepository.Add(optionValue);
                valueMap[value] = optionValue;
            }

            optionValueMap[optionDto.Name] = valueMap;
        }

        // 10. Create ProductVariant and ProductVariantOptionValue entities
        foreach (var variantDto in command.Variants)
        {
            // Create ProductVariant
            var productVariant = ProductVariant.Create(
                productId: product.Id,
                sku: variantDto.Sku,
                price: variantDto.Price,
                stock: variantDto.Stock,
                weight: variantDto.Weight,
                height: variantDto.Height,
                width: variantDto.Width,
                length: variantDto.Length
            );

            _productVariantRepository.Add(productVariant);

            // Create ProductVariantOptionValue (link variant with option values)
            for (int i = 0; i < variantDto.OptionValues.Count; i++)
            {
                string optionValue = variantDto.OptionValues[i];
                string optionName = command.Options[i].Name;

                // Get the ProductOptionValue from our map
                var productOptionValue = optionValueMap[optionName][optionValue];

                var variantOptionValue = ProductVariantOptionValue.Create(
                    variantId: productVariant.Id,
                    optionValueId: productOptionValue.Id
                );

                _productVariantOptionValueRepository.Add(variantOptionValue);
            }

            // Create ProductMedia for variant (if any)
            if (variantDto.VariantMedia != null && variantDto.VariantMedia.Count > 0)
            {
                foreach (var variantMediaDto in variantDto.VariantMedia)
                {
                    // Validate variant media file exists
                    bool variantMediaExists = await _fileStorageService.FileExistsAsync(variantMediaDto.MediaUrl);
                    if (!variantMediaExists)
                    {
                        return Result.Failure<Guid>(ProductErrors.MediaNotFound(variantMediaDto.MediaUrl));
                    }

                    var variantMediaType = variantMediaDto.MediaType.Equals("Video", StringComparison.OrdinalIgnoreCase)
                        ? MediaType.Video
                        : MediaType.Image;

                    var variantProductMedia = ProductMedia.CreateProductVariantMedia(
                        productId: product.Id,
                        productVariantId: productVariant.Id,
                        mediaUrl: variantMediaDto.MediaUrl,
                        mediaType: variantMediaType,
                        sortOrder: variantMediaDto.SortOrder,
                        isCover: variantMediaDto.IsCover
                    );

                    _productMediaRepository.Add(variantProductMedia);
                }
            }
        }

        // 11. Save all changes in a transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}

