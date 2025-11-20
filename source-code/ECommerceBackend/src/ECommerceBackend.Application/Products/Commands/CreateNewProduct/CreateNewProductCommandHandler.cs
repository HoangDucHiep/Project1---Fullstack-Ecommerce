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

            // 3. Validate and resolve all images and video
            List<Media> imageMediaList = await ResolveImagesAsync(request.Images, cancellationToken);
            Media? videoMedia = request.Video != null 
                ? await ResolveVideoAsync(request.Video, cancellationToken) 
                : null;

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

            // 6. Create product media (main product images + video)
            var productMedias = new List<ProductMedia>();
            
            // 6a. Add video FIRST (if exists) - Video luôn hiển thị đầu tiên với sortOrder = -1
            if (videoMedia != null && request.Video != null)
            {
                // Auto-confirm media when linking to product
                if (videoMedia.IsTemp)
                {
                    videoMedia.Confirm();
                    _mediaRepository.Update(videoMedia);
                }

                var productMedia = ProductMedia.CreateForProduct(
                    productId: product.Id,
                    mediaId: videoMedia.Id,
                    isCover: false, // Video không bao giờ là cover
                    sortOrder: -1 // Video luôn ở đầu tiên (sortOrder = -1)
                );
                await _productMediaRepository.AddAsync(productMedia, cancellationToken);
                productMedias.Add(productMedia);
            }
            
            // 6b. Add images AFTER video (sortOrder >= 0)
            foreach ((CreateProductImageDto imageDto, Media media) in request.Images.Zip(imageMediaList))
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
                    isCover: imageDto.IsCover,
                    sortOrder: imageDto.SortOrder // Images: sortOrder >= 0
                );
                await _productMediaRepository.AddAsync(productMedia, cancellationToken);
                productMedias.Add(productMedia);
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

            // Create variant images if specified
            if (variantDto.Images != null && variantDto.Images.Any())
            {
                await CreateVariantImages(variantDto, product.Id, variant.Id, cancellationToken);
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

    private async Task CreateVariantImages(
        CreateProductVariantDto variantDto,
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        List<Media> variantImageMediaList = await ResolveVariantImagesAsync(variantDto.Images!, cancellationToken);

        foreach ((CreateProductImageDto imageDto, Media media) in variantDto.Images!.Zip(variantImageMediaList))
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
                isCover: imageDto.IsCover,
                sortOrder: imageDto.SortOrder // Variant chỉ có ảnh (sortOrder >= 0)
            );
            await _productMediaRepository.AddAsync(productMedia, cancellationToken);
        }
    }

    /// <summary>
    /// Resolves product image DTOs to actual Media entities (must be Image type)
    /// </summary>
    private async Task<List<Media>> ResolveImagesAsync(List<CreateProductImageDto> imageDtos, CancellationToken cancellationToken)
    {
        var mediaList = new List<Media>();
        var missingMediaInfo = new List<string>();

        foreach (CreateProductImageDto imageDto in imageDtos)
        {
            Media? media = await _mediaRepository.GetByFileUrlAsync(imageDto.ImageUrl, cancellationToken);
            
            if (media == null)
            {
                missingMediaInfo.Add(imageDto.ImageUrl);
            }
            else if (media.MediaType != MediaType.Image)
            {
                throw new ApplicationInvalidOperationException(
                    Error.Validation("Media.InvalidType", $"Media {imageDto.ImageUrl} không phải là ảnh")
                );
            }
            else
            {
                mediaList.Add(media);
            }
        }

        if (missingMediaInfo.Any())
        {
            throw new ApplicationInvalidOperationException(
                Error.NotFound("Media.NotFound", $"Không tìm thấy ảnh: {string.Join(", ", missingMediaInfo)}")
            );
        }

        return mediaList;
    }

    /// <summary>
    /// Resolves variant image DTOs to actual Media entities (must be Image type)
    /// </summary>
    private async Task<List<Media>> ResolveVariantImagesAsync(List<CreateProductImageDto> imageDtos, CancellationToken cancellationToken)
    {
        return await ResolveImagesAsync(imageDtos, cancellationToken);
    }

    /// <summary>
    /// Resolves product video DTO to actual Media entity (must be Video type)
    /// </summary>
    private async Task<Media> ResolveVideoAsync(CreateProductVideoDto videoDto, CancellationToken cancellationToken)
    {
        Media? media = await _mediaRepository.GetByFileUrlAsync(videoDto.VideoUrl, cancellationToken);
        
        if (media == null)
        {
            throw new ApplicationInvalidOperationException(
                Error.NotFound("Media.NotFound", $"Không tìm thấy video: {videoDto.VideoUrl}")
            );
        }

        if (media.MediaType != MediaType.Video)
        {
            throw new ApplicationInvalidOperationException(
                Error.Validation("Media.InvalidType", $"Media {videoDto.VideoUrl} không phải là video")
            );
        }

        return media;
    }
}
