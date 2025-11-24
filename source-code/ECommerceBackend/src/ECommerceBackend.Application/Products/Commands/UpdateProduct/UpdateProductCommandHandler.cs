using ECommerceBackend.Application.Abstracts;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Services;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Abstracts.Utils;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Medias;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Products.Commands.UpdateProduct;

/// HDHiep - 11/23/2024
/// <summary>
/// Handler for updating an existing product
/// </summary>
public sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, ProductDetailDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductOptionTypeRepository _productOptionTypeRepository;
    private readonly IProductOptionValueRepository _productOptionValueRepository;
    private readonly IProductVariantOptionValueRepository _productVariantOptionValueRepository;
    private readonly IProductMediaRepository _productMediaRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductDetailAssembler _assembler;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IProductVariantRepository productVariantRepository,
        IProductOptionTypeRepository productOptionTypeRepository,
        IProductOptionValueRepository productOptionValueRepository,
        IProductVariantOptionValueRepository productVariantOptionValueRepository,
        IProductMediaRepository productMediaRepository,
        IMediaRepository mediaRepository,
        ICategoryRepository categoryRepository,
        IUserContext userContext,
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
        _categoryRepository = categoryRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _assembler = assembler;
    }

    public async Task<Result<ProductDetailDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // ===== BƯỚC 1: Load và validate Product =====
            Product? product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                throw new ApplicationInvalidOperationException(ProductErrors.NotFound(request.Id));
            }

            // Authorization check - verify seller owns this product's shop
            // TODO: Implement proper authorization when Shop-User relationship is ready
            // For now, skip this check

            // Validate product not locked
            if (product.Status == ProductStatus.Locked)
            {
                throw new ApplicationInvalidOperationException(ProductErrors.ProductLocked);
            }

            // Validate status transition for Seller
            ProductStatus[] allowedStatuses = [ProductStatus.Active, ProductStatus.Inactive, ProductStatus.OutOfStock];
            if (!allowedStatuses.Contains(request.Status))
            {
                throw new ApplicationInvalidOperationException(ProductErrors.InvalidStatusTransition);
            }

            // ===== BƯỚC 2: Validate Category exists =====
            Category? category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new ApplicationInvalidOperationException(CategoryErrors.NotFound(request.CategoryId));
            }

            // ===== BƯỚC 3: Validate SKU unique (if changed) =====
            if (product.Sku != request.Sku)
            {
                Product? existingProduct = await _productRepository.GetBySkuAsync(
                    request.Sku, product.ShopId, cancellationToken);

                if (existingProduct != null)
                {
                    throw new ApplicationInvalidOperationException(
                        ProductErrors.SkuDuplicate(request.Sku)
                    );
                }
            }

            // ===== BƯỚC 4: Update Basic Info =====
            string slug = SlugGenerator.GenerateSlug(request.Name);
            Product? existingSlugProduct = await _productRepository.GetBySlugAsync(slug, cancellationToken);
            if (existingSlugProduct != null && existingSlugProduct.Id != product.Id)
            {
                // Make slug unique if conflict
                slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
            }

            product.UpdateBasicInfo(request.Name, request.Description, slug, request.Sku, request.CategoryId);
            product.UpdateStatus(request.Status);
            _productRepository.Update(product);

            // ===== BƯỚC 5: Determine product type =====
            bool isComplexProduct = request.Options.Any();

            if (isComplexProduct)
            {
                // Complex product: Update options, variants, and their media
                await UpdateComplexProduct(request, product, cancellationToken);
            }
            else
            {
                // Simple product: Update ghost variant
                await UpdateSimpleProduct(request, product, cancellationToken);
            }

            // ===== BƯỚC 6: Update product media (Images + Video) =====
            await UpdateProductMedia(request, product.Id, cancellationToken);

            // ===== BƯỚC 7: Save all changes =====
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ===== BƯỚC 8: Build response DTO =====
            return await BuildProductDetailDto(product.Id, isComplexProduct, cancellationToken);
        }, cancellationToken);
    }

    private async Task UpdateComplexProduct(
        UpdateProductCommand request,
        Product product,
        CancellationToken cancellationToken)
    {
        // Load existing options, values, variants
        List<ProductOptionType> existingOptions = await _productOptionTypeRepository.GetByProductIdAsync(product.Id, cancellationToken);
        List<ProductVariant> existingVariants = await _productVariantRepository.GetByProductIdAsync(product.Id, cancellationToken);

        // Create dictionaries for quick lookup
        var existingOptionsDict = existingOptions.ToDictionary(o => o.Id);
        var existingOptionsByName = existingOptions.ToDictionary(o => o.Name);
        var existingVariantsDict = existingVariants.ToDictionary(v => v.Id);

        // Track which entities are still in use
        var processedOptionIds = new HashSet<Guid>();
        var processedVariantIds = new HashSet<Guid>();

        // ===== 1. Update/Create Options and Values =====
        var optionValueMap = new Dictionary<string, Dictionary<string, ProductOptionValue>>();

        foreach (UpdateProductOptionDto optionDto in request.Options)
        {
            ProductOptionType option;

            // Try to find existing option by Id first, then by Name
            ProductOptionType? existingOption = null;

            if (optionDto.Id.HasValue && existingOptionsDict.TryGetValue(optionDto.Id.Value, out var existingById))
            {
                existingOption = existingById;
            }
            else if (existingOptionsByName.TryGetValue(optionDto.Name, out var existingByName))
            {
                existingOption = existingByName;
            }

            if (existingOption != null)
            {
                // Update existing option
                existingOption.UpdateName(optionDto.Name);
                option = existingOption;
                processedOptionIds.Add(option.Id);
            }
            else
            {
                // Create new option
                option = ProductOptionType.Create(product.Id, optionDto.Name);
                await _productOptionTypeRepository.AddAsync(option, cancellationToken);
                processedOptionIds.Add(option.Id);
            }

            // Update/Create option values
            List<ProductOptionValue> existingValues = await _productOptionValueRepository.GetByProductOptionTypeIdAsync(option.Id, cancellationToken);
            var existingValuesDict = existingValues.ToDictionary(v => v.Id);
            var existingValuesByValue = existingValues.ToDictionary(v => v.Value);
            var processedValueIds = new HashSet<Guid>();

            var optionValues = new Dictionary<string, ProductOptionValue>();

            foreach (UpdateProductOptionValueDto valueDto in optionDto.Values)
            {
                ProductOptionValue optionValue;

                // Try to find existing value by Id first, then by Value
                ProductOptionValue? existingValue = null;

                if (valueDto.Id.HasValue && existingValuesDict.TryGetValue(valueDto.Id.Value, out var existingValueById))
                {
                    existingValue = existingValueById;
                }
                else if (existingValuesByValue.TryGetValue(valueDto.Value, out var existingValueByValue))
                {
                    existingValue = existingValueByValue;
                }

                if (existingValue != null)
                {
                    // Update existing value
                    existingValue.UpdateValue(valueDto.Value);
                    optionValue = existingValue;
                    processedValueIds.Add(optionValue.Id);
                }
                else
                {
                    // Create new value
                    optionValue = ProductOptionValue.Create(option.Id, valueDto.Value);
                    await _productOptionValueRepository.AddAsync(optionValue, cancellationToken);
                    processedValueIds.Add(optionValue.Id);
                }

                optionValues[valueDto.Value] = optionValue;
            }

            optionValueMap[optionDto.Name] = optionValues;

            // Soft delete unused values
            foreach (var value in existingValues)
            {
                if (!processedValueIds.Contains(value.Id))
                {
                    value.MarkAsDeleted();
                }
            }
        }

        // Soft delete unused options (and cascade to values)
        foreach (var option in existingOptions)
        {
            if (!processedOptionIds.Contains(option.Id))
            {
                option.MarkAsDeleted();
            }
        }

        // ===== 2. Validate Variant SKUs =====
        var variantSkus = request.Variants
            .Select(v => v.Sku)
            .ToList();

        // Check duplicates within request
        var duplicates = variantSkus
            .GroupBy(s => s)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Any())
        {
            throw new ApplicationInvalidOperationException(
                ProductErrors.VariantSkuDuplicateInRequest(string.Join(", ", duplicates))
            );
        }

        // Check duplicates in database (excluding current product's variants)
        foreach (string variantSku in variantSkus)
        {
            ProductVariant? existingVariant = await _productVariantRepository.GetBySkuAsync(
                variantSku, product.ShopId, cancellationToken);

            if (existingVariant != null && existingVariant.ProductId != product.Id)
            {
                throw new ApplicationInvalidOperationException(
                    ProductErrors.VariantSkuDuplicate(variantSku)
                );
            }
        }

        // ===== 3. Update/Create Variants =====
        foreach (UpdateProductVariantDto variantDto in request.Variants)
        {
            ProductVariant variant;

            if (variantDto.Id.HasValue && existingVariantsDict.TryGetValue(variantDto.Id.Value, out var existingVariant))
            {
                // Update existing variant
                existingVariant.UpdateDetails(
                    sku: variantDto.Sku,
                    price: variantDto.Price,
                    stock: variantDto.Stock,
                    weight: variantDto.Weight ?? 0,
                    height: variantDto.Height ?? 0,
                    width: variantDto.Width ?? 0,
                    length: variantDto.Length ?? 0
                );
                existingVariant.UpdateStatus(variantDto.Status);
                variant = existingVariant;
                processedVariantIds.Add(variant.Id);
            }
            else
            {
                // Create new variant
                variant = ProductVariant.Create(
                    productId: product.Id,
                    sku: variantDto.Sku,
                    price: variantDto.Price,
                    stock: variantDto.Stock,
                    weight: variantDto.Weight ?? 0,
                    height: variantDto.Height ?? 0,
                    width: variantDto.Width ?? 0,
                    length: variantDto.Length ?? 0
                );
                variant.UpdateStatus(variantDto.Status);
                await _productVariantRepository.AddAsync(variant, cancellationToken);
                processedVariantIds.Add(variant.Id);
            }

            // Update variant option values
            await UpdateVariantOptionValues(variant, variantDto.OptionValues, optionValueMap, request.Options, cancellationToken);

            // Update variant images
            if (variantDto.Images != null)
            {
                await UpdateVariantImages(variant.Id, product.Id, variantDto.Images, cancellationToken);
            }
        }

        // Soft delete unused variants (with cascade delete)
        foreach (var variant in existingVariants)
        {
            if (!processedVariantIds.Contains(variant.Id))
            {
                variant.MarkAsDeleted();
                
                // CASCADE: Xóa variant option values
                List<ProductVariantOptionValue> vovs = await _productVariantOptionValueRepository
                    .GetByVariantIdAsync(variant.Id, cancellationToken);
                foreach (var vov in vovs)
                {
                    vov.MarkAsDeleted();
                }
                
                // CASCADE: Xóa variant images
                List<ProductMedia> allMedias = await _productMediaRepository
                    .GetByProductIdAsync(product.Id, cancellationToken);
                var variantImages = allMedias.Where(m => m.ProductVariantId == variant.Id);
                foreach (var img in variantImages)
                {
                    img.MarkAsDeleted();
                }
            }
        }
    }

    private async Task UpdateSimpleProduct(
        UpdateProductCommand request,
        Product product,
        CancellationToken cancellationToken)
    {
        // Load existing variants (should be 1 ghost variant)
        List<ProductVariant> existingVariants = await _productVariantRepository.GetByProductIdAsync(product.Id, cancellationToken);

        if (existingVariants.Count == 0)
        {
            // Create ghost variant if not exists
            var defaultVariant = ProductVariant.Create(
                productId: product.Id,
                sku: $"DEFAULT-{Guid.NewGuid().ToString()[..8]}",
                price: request.DefaultPrice!.Value,
                stock: request.DefaultStock!.Value,
                weight: request.DefaultWeight ?? 0,
                height: request.DefaultHeight ?? 0,
                width: request.DefaultWidth ?? 0,
                length: request.DefaultLength ?? 0
            );
            await _productVariantRepository.AddAsync(defaultVariant, cancellationToken);
        }
        else
        {
            // Update ghost variant
            var ghostVariant = existingVariants.First();
            ghostVariant.UpdateDetails(
                sku: ghostVariant.Sku, // Don't change SKU
                price: request.DefaultPrice!.Value,
                stock: request.DefaultStock!.Value,
                weight: request.DefaultWeight ?? 0,
                height: request.DefaultHeight ?? 0,
                width: request.DefaultWidth ?? 0,
                length: request.DefaultLength ?? 0
            );
        }
    }

    private async Task UpdateProductMedia(
        UpdateProductCommand request,
        Guid productId,
        CancellationToken cancellationToken)
    {
        // Load existing product media (excluding variant media)
        List<ProductMedia> existingMedias = await _productMediaRepository.GetByProductIdAsync(productId, cancellationToken);
        var existingProductMedias = existingMedias.Where(m => m.ProductVariantId == null).ToList();

        var processedUrls = new HashSet<string>();

        // ===== 1. Update/Create Video (sortOrder = -1) =====
        var existingVideo = existingProductMedias.FirstOrDefault(m => m.IsVideo());

        if (request.Video != null && !string.IsNullOrWhiteSpace(request.Video.VideoUrl))
        {
            Media? videoMedia = await _mediaRepository.GetByFileUrlAsync(request.Video.VideoUrl, cancellationToken);
            if (videoMedia == null)
            {
                throw new ApplicationInvalidOperationException(
                    Error.NotFound("Media.NotFound", $"Không tìm thấy video: {request.Video.VideoUrl}")
                );
            }

            if (videoMedia.MediaType != MediaType.Video)
            {
                throw new ApplicationInvalidOperationException(
                    Error.Validation("Media.InvalidType", $"Media {request.Video.VideoUrl} không phải là video")
                );
            }

            // Check if URL same as existing video
            if (existingVideo != null && existingVideo.Media.FileUrl == request.Video.VideoUrl)
            {
                // Same URL - keep existing
                processedUrls.Add(request.Video.VideoUrl);
            }
            else
            {
                // Different URL or no existing video - recreate
                existingVideo?.MarkAsDeleted();

                var newVideo = ProductMedia.CreateForProduct(
                    productId: productId,
                    mediaId: videoMedia.Id,
                    isCover: false,
                    sortOrder: -1
                );
                await _productMediaRepository.AddAsync(newVideo, cancellationToken);
                processedUrls.Add(request.Video.VideoUrl);

                if (videoMedia.IsTemp)
                {
                    videoMedia.Confirm();
                    _mediaRepository.Update(videoMedia);
                }
            }
        }
        else
        {
            // Delete existing video if no video in request
            existingVideo?.MarkAsDeleted();
        }

        // ===== 2. Update/Create Images (sortOrder >= 0) =====
        var existingImages = existingProductMedias.Where(m => m.IsImage()).ToList();

        foreach (UpdateProductImageDto imageDto in request.Images)
        {
            Media? imageMedia = await _mediaRepository.GetByFileUrlAsync(imageDto.ImageUrl, cancellationToken);
            if (imageMedia == null)
            {
                throw new ApplicationInvalidOperationException(
                    Error.NotFound("Media.NotFound", $"Không tìm thấy ảnh: {imageDto.ImageUrl}")
                );
            }

            if (imageMedia.MediaType != MediaType.Image)
            {
                throw new ApplicationInvalidOperationException(
                    Error.Validation("Media.InvalidType", $"Media {imageDto.ImageUrl} không phải là ảnh")
                );
            }

            // Find existing image by URL
            var existingImage = existingImages.FirstOrDefault(img => img.Media.FileUrl == imageDto.ImageUrl);

            if (existingImage != null)
            {
                // URL exists - update metadata only
                existingImage.UpdateCoverStatus(imageDto.IsCover);
                existingImage.UpdateSortOrder(imageDto.SortOrder);
                processedUrls.Add(imageDto.ImageUrl);
            }
            else
            {
                // New URL - create new image
                var newImage = ProductMedia.CreateForProduct(
                    productId: productId,
                    mediaId: imageMedia.Id,
                    isCover: imageDto.IsCover,
                    sortOrder: imageDto.SortOrder
                );
                await _productMediaRepository.AddAsync(newImage, cancellationToken);
                processedUrls.Add(imageDto.ImageUrl);

                if (imageMedia.IsTemp)
                {
                    imageMedia.Confirm();
                    _mediaRepository.Update(imageMedia);
                }
            }
        }

        // Soft delete images not in request
        foreach (var image in existingImages)
        {
            if (!processedUrls.Contains(image.Media.FileUrl))
            {
                image.MarkAsDeleted();
            }
        }
    }

    private async Task UpdateVariantImages(
        Guid variantId,
        Guid productId,
        List<UpdateProductImageDto> imageDtos,
        CancellationToken cancellationToken)
    {
        // Load existing variant media
        List<ProductMedia> existingMedias = await _productMediaRepository.GetByProductIdAsync(productId, cancellationToken);
        var existingVariantImages = existingMedias.Where(m => m.ProductVariantId == variantId && m.IsImage()).ToList();

        var processedUrls = new HashSet<string>();

        foreach (UpdateProductImageDto imageDto in imageDtos)
        {
            Media? imageMedia = await _mediaRepository.GetByFileUrlAsync(imageDto.ImageUrl, cancellationToken);
            if (imageMedia == null)
            {
                throw new ApplicationInvalidOperationException(
                    Error.NotFound("Media.NotFound", $"Không tìm thấy ảnh: {imageDto.ImageUrl}")
                );
            }

            if (imageMedia.MediaType != MediaType.Image)
            {
                throw new ApplicationInvalidOperationException(
                    Error.Validation("Media.InvalidType", $"Media {imageDto.ImageUrl} không phải là ảnh")
                );
            }

            // Find existing image by URL
            var existingImage = existingVariantImages.FirstOrDefault(img => img.Media.FileUrl == imageDto.ImageUrl);

            if (existingImage != null)
            {
                // URL exists - update metadata only
                existingImage.UpdateCoverStatus(imageDto.IsCover);
                existingImage.UpdateSortOrder(imageDto.SortOrder);
                processedUrls.Add(imageDto.ImageUrl);
            }
            else
            {
                // New URL - create new image
                var newImage = ProductMedia.CreateForVariant(
                    productId: productId,
                    productVariantId: variantId,
                    mediaId: imageMedia.Id,
                    isCover: imageDto.IsCover,
                    sortOrder: imageDto.SortOrder
                );
                await _productMediaRepository.AddAsync(newImage, cancellationToken);
                processedUrls.Add(imageDto.ImageUrl);

                if (imageMedia.IsTemp)
                {
                    imageMedia.Confirm();
                    _mediaRepository.Update(imageMedia);
                }
            }
        }

        // Soft delete images not in request
        foreach (var image in existingVariantImages)
        {
            if (!processedUrls.Contains(image.Media.FileUrl))
            {
                image.MarkAsDeleted();
            }
        }
    }

    private async Task UpdateVariantOptionValues(
        ProductVariant variant,
        List<string> optionValues,
        Dictionary<string, Dictionary<string, ProductOptionValue>> optionValueMap,
        List<UpdateProductOptionDto> options,
        CancellationToken cancellationToken)
    {
        // Load existing variant option values
        List<ProductVariantOptionValue> existingVOVs = await _productVariantOptionValueRepository.GetByVariantIdAsync(variant.Id, cancellationToken);

        // Build target option value IDs from request
        var targetOptionValueIds = new HashSet<Guid>();
        for (int i = 0; i < optionValues.Count && i < options.Count; i++)
        {
            string optionValue = optionValues[i];
            string optionName = options[i].Name;

            if (optionValueMap.TryGetValue(optionName, out Dictionary<string, ProductOptionValue>? values) &&
                values.TryGetValue(optionValue, out ProductOptionValue? productOptionValue))
            {
                targetOptionValueIds.Add(productOptionValue.Id);
            }
        }

        // Soft delete VOVs that are not in target set
        foreach (var vov in existingVOVs)
        {
            if (!targetOptionValueIds.Contains(vov.OptionValueId))
            {
                vov.MarkAsDeleted();
            }
        }

        // Create new VOVs that don't exist yet
        var existingOptionValueIds = existingVOVs.Select(vov => vov.OptionValueId).ToHashSet();
        foreach (var targetOptionValueId in targetOptionValueIds)
        {
            if (!existingOptionValueIds.Contains(targetOptionValueId))
            {
                var variantOptionValue = ProductVariantOptionValue.Create(
                    variantId: variant.Id,
                    optionValueId: targetOptionValueId
                );
                await _productVariantOptionValueRepository.AddAsync(variantOptionValue, cancellationToken);
            }
        }
    }

    private async Task<ProductDetailDto> BuildProductDetailDto(
        Guid productId,
        bool isComplexProduct,
        CancellationToken cancellationToken)
    {
        // Reload product with all related data
        Product product = (await _productRepository.GetByIdAsync(productId, cancellationToken))!;
        List<ProductMedia> productMedias = await _productMediaRepository.GetByProductIdAsync(productId, cancellationToken);
        var mainProductMedias = productMedias.Where(m => m.ProductVariantId == null).ToList();

        var mediaDtos = await _assembler.BuildMediaDtos(mainProductMedias, cancellationToken);

        if (isComplexProduct)
        {
            List<ProductOptionType> options = await _productOptionTypeRepository.GetByProductIdAsync(productId, cancellationToken);
            List<ProductVariant> variants = await _productVariantRepository.GetByProductIdAsync(productId, cancellationToken);

            var optionDtos = await _assembler.BuildOptionDtos(options, cancellationToken);
            var variantDtos = await _assembler.BuildVariantDtos(variants, cancellationToken);

            return _assembler.BuildProductDetail(
                product,
                mediaDtos,
                options,
                variants,
                optionDtos,
                variantDtos,
                simpleOverrides: null
            );
        }
        else
        {
            List<ProductVariant> variants = await _productVariantRepository.GetByProductIdAsync(productId, cancellationToken);
            ProductVariant? ghostVariant = variants.FirstOrDefault();

            ProductDetailAssembler.SimpleOverrides? overrides = ghostVariant != null
                ? new ProductDetailAssembler.SimpleOverrides(
                    Price: (decimal?)ghostVariant.Price,
                    Stock: ghostVariant.Stock,
                    Weight: (decimal?)ghostVariant.Weight,
                    Height: (decimal?)ghostVariant.Height,
                    Width: (decimal?)ghostVariant.Width,
                    Length: (decimal?)ghostVariant.Length
                )
                : null;

            return _assembler.BuildProductDetail(
                product,
                mediaDtos,
                productOptions: new List<ProductOptionType>(),
                productVariants: variants,
                optionDtos: null,
                variantDtos: null,
                simpleOverrides: overrides
            );
        }
    }
}
