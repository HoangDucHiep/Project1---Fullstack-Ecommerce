using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Media;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Services;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Abstracts.Utils;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Medias;
using ECommerceBackend.Domain.Products;
using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Products.Commands.CreateNewProduct;

/// HDHiep - 10/28/2025
/// <summary>
/// Handler for creating a new product
/// </summary>
public class CreateNewProductCommandHandler : ICommandHandler<CreateNewProductCommand, ProductDetailDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductOptionTypeRepository _productOptionTypeRepository;
    private readonly IProductOptionValueRepository _productOptionValueRepository;
    private readonly IProductVariantOptionValueRepository _productVariantOptionValueRepository;
    private readonly IProductMediaRepository _productMediaRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IShopRepository _shopRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductDetailAssembler _assembler;

    public CreateNewProductCommandHandler(
        IProductRepository productRepository,
        IProductVariantRepository productVariantRepository,
        IProductOptionTypeRepository productOptionTypeRepository,
        IProductOptionValueRepository productOptionValueRepository,
        IProductVariantOptionValueRepository productVariantOptionValueRepository,
        IProductMediaRepository productMediaRepository,
        IMediaRepository mediaRepository,
        IShopRepository shopRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ProductDetailAssembler assembler)
    {
        _productRepository = productRepository;
        _productVariantRepository = productVariantRepository;
        _productOptionTypeRepository = productOptionTypeRepository;
        _productOptionValueRepository = productOptionValueRepository;
        _productVariantOptionValueRepository = productVariantOptionValueRepository;
        _productMediaRepository = productMediaRepository;
        _mediaRepository = mediaRepository;
        _shopRepository = shopRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _assembler = assembler;
    }

    public async Task<Result<ProductDetailDto>> Handle(CreateNewProductCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // 1. Validate shop exists
            Shop? shop = await _shopRepository.GetByIdAsync(request.ShopId, cancellationToken);
            if (shop == null)
            {
                throw new ApplicationInvalidOperationException(ShopErrors.NotFound(request.ShopId));
            }

            // 2. Validate category exists
            Category? category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new ApplicationInvalidOperationException(CategoryErrors.NotFound(request.CategoryId));
            }

            // 3. Validate and resolve all medias
            List<Media> mediaList = await ResolveMediasAsync(request.Medias, cancellationToken);

            // 4. Generate unique slug, create product entity
            string slug = SlugGenerator.GenerateSlug(request.Name);
            var product = Product.Create(
                shopId: request.ShopId,
                categoryId: request.CategoryId,
                name: request.Name,
                description: request.Description,
                slug: slug,
                sku: request.Sku, // Product SKU
                status: ProductStatus.Active);

            await _productRepository.AddAsync(product, cancellationToken);

            var productOptions = new List<ProductOptionType>();
            var productVariants = new List<ProductVariant>();

            // 5. Determine product type and create accordingly
            if (request.Options.Any())
            {
                // Complex product with options and variants
                await CreateComplexProduct(request, product, productOptions, productVariants, cancellationToken);
            }
            else
            {
                // Simple product with ghost variant
                await CreateSimpleProduct(request, product, productVariants, cancellationToken);
            }

            // 6. Create product media (main product images)
            var productMedias = new List<ProductMedia>();
            foreach ((CreateProductMediaDto mediaDto, Media media) in request.Medias.Zip(mediaList))
            {
                // Auto-confirm media when linking to product
                if (media.IsTemp)
                {
                    media.Confirm();
                    _mediaRepository.Update(media);
                }

                var productMedia = ProductMedia.CreateForProduct(
                    productId: product.Id,
                    mediaId: media.Id,
                    isCover: mediaDto.IsCover,
                    sortOrder: mediaDto.SortOrder
                );
                productMedias.Add(productMedia);
                await _productMediaRepository.AddAsync(productMedia, cancellationToken);
            }

            // 7. Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 8. Build response DTO
            List<ProductMediaDto> mediaDtos = await _assembler.BuildMediaDtos(productMedias, cancellationToken);

            ProductDetailDto productDetailDto;
            if (productOptions.Any())
            {
                // Complex product with variants
                List<ProductOptionDto> optionDtos = await _assembler.BuildOptionDtos(productOptions, cancellationToken);
                List<ProductVariantDetailDto> variantDtos = await _assembler.BuildVariantDtos(productVariants, cancellationToken);

                productDetailDto = _assembler.BuildProductDetail(
                    product,
                    mediaDtos,
                    productOptions,
                    productVariants,
                    optionDtos,
                    variantDtos,
                    simpleOverrides: null
                );
            }
            else
            {
                // Simple product - use data from request
                var overrides = new ProductDetailAssembler.SimpleOverrides(
                    Price: (decimal?)request.DefaultPrice,
                    Stock: request.DefaultStock,
                    Weight: (decimal?)request.DefaultWeight,
                    Height: (decimal?)request.DefaultHeight,
                    Width: (decimal?)request.DefaultWidth,
                    Length: (decimal?)request.DefaultLength
                );

                productDetailDto = _assembler.BuildProductDetail(
                    product,
                    mediaDtos,
                    productOptions,
                    productVariants,
                    optionDtos: null,
                    variantDtos: null,
                    simpleOverrides: overrides
                );
            }

            return productDetailDto;
        }, cancellationToken);
    }

    private async Task CreateComplexProduct(
        CreateNewProductCommand request,
        Product product,
        List<ProductOptionType> productOptions,
        List<ProductVariant> productVariants,
        CancellationToken cancellationToken)
    {
        // Create options and option values
        var optionValueMap = new Dictionary<string, Dictionary<string, ProductOptionValue>>();

        foreach (CreateProductOptionDto optionDto in request.Options)
        {
            var option = ProductOptionType.Create(
                productId: product.Id,
                name: optionDto.Name);

            productOptions.Add(option);
            await _productOptionTypeRepository.AddAsync(option, cancellationToken);

            // Create option values for this option type
            var optionValues = new Dictionary<string, ProductOptionValue>();
            foreach (string value in optionDto.Values)
            {
                var optionValue = ProductOptionValue.Create(option.Id, value);
                optionValues[value] = optionValue;
                await _productOptionValueRepository.AddAsync(optionValue, cancellationToken);
            }
            optionValueMap[optionDto.Name] = optionValues;
        }

        // Create variants
        foreach (CreateProductVariantDto variantDto in request.Variants)
        {
            var variant = ProductVariant.Create(
                productId: product.Id,
                sku: variantDto.Sku ?? $"SKU-{Guid.NewGuid().ToString()[..8]}",
                price: variantDto.Price,
                stock: variantDto.Stock,
                weight: (double)(variantDto.Weight ?? 0),
                height: (double)(variantDto.Height ?? 0),
                width: (double)(variantDto.Width ?? 0),
                length: (double)(variantDto.Length ?? 0)
            );

            productVariants.Add(variant);
            await _productVariantRepository.AddAsync(variant, cancellationToken);

            // Create variant option values
            for (int i = 0; i < variantDto.OptionValues.Count && i < request.Options.Count; i++)
            {
                string optionValue = variantDto.OptionValues[i];
                string optionName = request.Options[i].Name;

                if (optionValueMap.TryGetValue(optionName, out Dictionary<string, ProductOptionValue>? optionValues) &&
                    optionValues.TryGetValue(optionValue, out ProductOptionValue? productOptionValue))
                {
                    var variantOptionValue = ProductVariantOptionValue.Create(
                        variantId: variant.Id,
                        optionValueId: productOptionValue.Id
                    );
                    await _productVariantOptionValueRepository.AddAsync(variantOptionValue, cancellationToken);
                }
            }

            // Create variant media if specified
            if (variantDto.Medias != null && variantDto.Medias.Any())
            {
                await CreateVariantMedia(variantDto, product.Id, variant.Id, cancellationToken);
            }
        }
    }

    private async Task CreateSimpleProduct(
        CreateNewProductCommand request,
        Product product,
        List<ProductVariant> productVariants,
        CancellationToken cancellationToken)
    {
        // Create ghost variant for simple product - always use DEFAULT format for SKU
        var defaultVariant = ProductVariant.Create(
            productId: product.Id,
            sku: $"DEFAULT-{Guid.NewGuid().ToString()[..8]}", // Always generate DEFAULT SKU for ghost variant
            price: request.DefaultPrice!.Value,
            stock: request.DefaultStock!.Value,
            weight: request.DefaultWeight ?? 0,
            height: request.DefaultHeight ?? 0,
            width: request.DefaultWidth ?? 0,
            length: request.DefaultLength ?? 0
        );

        productVariants.Add(defaultVariant);
        await _productVariantRepository.AddAsync(defaultVariant, cancellationToken);
    }

    private async Task CreateVariantMedia(
        CreateProductVariantDto variantDto,
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        List<Media> variantMediaList = await ResolveMediasAsync(variantDto.Medias!, cancellationToken);

        foreach ((CreateProductMediaDto mediaDto, Media media) in variantDto.Medias!.Zip(variantMediaList))
        {
            // Auto-confirm media when linking to product
            if (media.IsTemp)
            {
                media.Confirm();
                _mediaRepository.Update(media);
            }

            var productMedia = ProductMedia.CreateForVariant(
                productId: productId,
                productVariantId: variantId,
                mediaId: media.Id,
                isCover: mediaDto.IsCover,
                sortOrder: mediaDto.SortOrder
            );
            await _productMediaRepository.AddAsync(productMedia, cancellationToken);
        }
    }

    /// <summary>
    /// Resolves media DTOs to actual Media entities by name or URL
    /// </summary>
    private async Task<List<Media>> ResolveMediasAsync(List<CreateProductMediaDto> mediaDtos, CancellationToken cancellationToken)
    {
        var mediaList = new List<Media>();
        var missingMediaInfo = new List<string>();

        foreach (CreateProductMediaDto mediaDto in mediaDtos)
        {
            Media? media = null;

            // Try to find by filename first
            if (!string.IsNullOrWhiteSpace(mediaDto.MediaName))
            {
                media = await _mediaRepository.GetByFileNameAsync(mediaDto.MediaName, cancellationToken);
            }

            // If not found by name, try by URL
            if (media == null && !string.IsNullOrWhiteSpace(mediaDto.MediaUrl))
            {
                media = await _mediaRepository.GetByFileUrlAsync(mediaDto.MediaUrl, cancellationToken);
            }

            if (media == null)
            {
                string identifier = mediaDto.MediaName ?? mediaDto.MediaUrl ?? "unknown";
                missingMediaInfo.Add(identifier);
            }
            else
            {
                mediaList.Add(media);
            }
        }

        if (missingMediaInfo.Any())
        {
            throw new ApplicationInvalidOperationException(
                Error.NotFound("Media.NotFound", $"Media not found: {string.Join(", ", missingMediaInfo)}")
            );
        }

        return mediaList;
    }
}
