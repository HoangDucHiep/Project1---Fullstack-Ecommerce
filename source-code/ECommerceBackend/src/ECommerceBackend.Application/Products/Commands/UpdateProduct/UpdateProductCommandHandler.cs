using ECommerceBackend.Application.Abstracts;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Abstracts.Utils;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Medias;
using ECommerceBackend.Domain.Products;
using Microsoft.Extensions.Logging;

namespace ECommerceBackend.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, ProductDetailDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMediaRepository _productMediaRepository;
    private readonly IProductOptionTypeRepository _productOptionTypeRepository;
    private readonly IProductOptionValueRepository _productOptionValueRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductVariantOptionValueRepository _productVariantOptionValueRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IProductMediaRepository productMediaRepository,
        IProductOptionTypeRepository productOptionTypeRepository,
        IProductOptionValueRepository productOptionValueRepository,
        IProductVariantRepository productVariantRepository,
        IProductVariantOptionValueRepository productVariantOptionValueRepository,
        IMediaRepository mediaRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _productMediaRepository = productMediaRepository;
        _productOptionTypeRepository = productOptionTypeRepository;
        _productOptionValueRepository = productOptionValueRepository;
        _productVariantRepository = productVariantRepository;
        _productVariantOptionValueRepository = productVariantOptionValueRepository;
        _mediaRepository = mediaRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ProductDetailDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Load Product với Authorization checks =====
        Product? product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return Result.Failure<ProductDetailDto>(ProductErrors.NotFound(request.Id));
        }

        // TODO: Authorization check - verify seller owns this product's shop
        // var currentUserShopId = GetCurrentUserShopId(); // Get from ClaimsPrincipal
        // if (product.ShopId != currentUserShopId)
        //     return Result.Failure<ProductDetailDto>(ProductErrors.Unauthorized);

        // Status validation
        if (product.Status == ProductStatus.Locked)
        {
            return Result.Failure<ProductDetailDto>(ProductErrors.ProductLocked);
        }

        // Validate status transition for Seller
        ProductStatus[] allowedStatuses = [ProductStatus.Active, ProductStatus.Inactive, ProductStatus.OutOfStock];
        if (!allowedStatuses.Contains(request.Status))
        {
            return Result.Failure<ProductDetailDto>(ProductErrors.InvalidStatusTransition);
        }

        // ===== BƯỚC 2: Validate Category exists =====
        Category? category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            return Result.Failure<ProductDetailDto>(ProductErrors.CategoryNotFound(request.CategoryId));
        }

        // ===== BƯỚC 3: Update Basic Info =====
        string slug = SlugGenerator.GenerateSlug(request.Name);
        Product? existingSlugProduct = await _productRepository.GetBySlugAsync(slug, cancellationToken);
        if (existingSlugProduct != null && existingSlugProduct.Id != product.Id)
        {
            // Make slug unique if conflict
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
        }

        product.UpdateBasicInfo(request.Name, request.Description, slug, request.Sku, request.CategoryId);
        product.UpdateStatus(request.Status);

        // ===== BƯỚC 4: Update Media (Incremental) =====
        List<ProductMedia> existingMedias = await _productMediaRepository.GetByProductIdAsync(product.Id, cancellationToken);
        var existingMediasDict = existingMedias.Where(m => m.ProductVariantId == null).ToDictionary(m => m.Id);

        foreach (var mediaDto in request.Medias)
        {
            if (mediaDto.Id.HasValue)
            {
                // ===== UPDATE: Cập nhật metadata của media đã tồn tại =====
                if (existingMediasDict.TryGetValue(mediaDto.Id.Value, out var existingMedia))
                {
                    // Chỉ update IsCover và SortOrder
                    existingMedia.UpdateCoverStatus(mediaDto.IsCover);
                    existingMedia.UpdateSortOrder(mediaDto.SortOrder);
                    
                    // Đánh dấu đã xử lý
                    existingMediasDict.Remove(mediaDto.Id.Value);
                }
                // Nếu không tìm thấy Id trong DB → bỏ qua (có thể log warning)
            }
            else
            {
                // ===== CREATE: Thêm media mới =====
                if (string.IsNullOrWhiteSpace(mediaDto.MediaUrl))
                {
                    return Result.Failure<ProductDetailDto>(ProductErrors.MediaNotFound("MediaUrl is required"));
                }

                var mediaEntity = await _mediaRepository.GetByFileUrlAsync(mediaDto.MediaUrl, cancellationToken);
                if (mediaEntity == null)
                {
                    return Result.Failure<ProductDetailDto>(ProductErrors.MediaNotFound(mediaDto.MediaUrl));
                }

                // Tạo ProductMedia mới
                var productMedia = ProductMedia.CreateForProduct(
                    product.Id,
                    mediaEntity.Id,
                    mediaDto.IsCover,
                    mediaDto.SortOrder
                );
                await _productMediaRepository.AddAsync(productMedia, cancellationToken);
            }
        }

        // ===== DELETE: Soft delete các media không có trong request =====
        foreach (var unprocessedMedia in existingMediasDict.Values)
        {
            unprocessedMedia.MarkAsDeleted();
        }

        // ===== BƯỚC 5: Update Options & Values (Incremental) =====
        var existingOptions = await _productOptionTypeRepository.GetByProductIdAsync(product.Id, cancellationToken);
        var existingValues = await _productOptionValueRepository.GetByProductOptionTypeIdsAsync(
            existingOptions.Select(o => o.Id).ToList(),
            cancellationToken
        );

        var existingOptionsDict = existingOptions.ToDictionary(o => o.Id);
        var processedOptions = new Dictionary<string, ProductOptionType>();
        var processedValues = new Dictionary<Guid, List<ProductOptionValue>>();

        foreach (var optionDto in request.Options)
        {
            ProductOptionType optionType;

            if (optionDto.Id.HasValue)
            {
                // Update existing option
                if (existingOptionsDict.TryGetValue(optionDto.Id.Value, out var existingOption))
                {
                    existingOption.UpdateName(optionDto.Name);
                    optionType = existingOption;
                    existingOptionsDict.Remove(optionDto.Id.Value);
                }
                else
                {
                    return Result.Failure<ProductDetailDto>(ProductErrors.OptionNotFound(optionDto.Id.Value));
                }
            }
            else
            {
                // Create new option
                optionType = ProductOptionType.Create(product.Id, optionDto.Name);
                await _productOptionTypeRepository.AddAsync(optionType, cancellationToken);
            }

            processedOptions[optionDto.Name] = optionType;

            // Process values for this option
            var existingValuesForOption = existingValues.Where(v => v.ProductOptionTypeId == optionType.Id).ToList();
            var existingValuesDict = existingValuesForOption.ToDictionary(v => v.Id);
            var processedValuesForOption = new List<ProductOptionValue>();

            if (optionDto.Values != null)
            {
                foreach (var valueDto in optionDto.Values)
                {
                    ProductOptionValue optionValue;

                    if (valueDto.Id.HasValue)
                    {
                        // Update existing value
                        if (existingValuesDict.TryGetValue(valueDto.Id.Value, out var existingValue))
                        {
                            existingValue.UpdateValue(valueDto.Value);
                            optionValue = existingValue;
                            existingValuesDict.Remove(valueDto.Id.Value);
                        }
                        else
                        {
                            return Result.Failure<ProductDetailDto>(ProductErrors.OptionValueNotFound(valueDto.Id.Value));
                        }
                    }
                    else
                    {
                        // Create new value
                        optionValue = ProductOptionValue.Create(optionType.Id, valueDto.Value);
                        await _productOptionValueRepository.AddAsync(optionValue, cancellationToken);
                    }

                    processedValuesForOption.Add(optionValue);
                }
            }

            processedValues[optionType.Id] = processedValuesForOption;

            // Soft delete unprocessed values
            foreach (var unprocessedValue in existingValuesDict.Values)
            {
                unprocessedValue.MarkAsDeleted();
                // CASCADE: Soft delete variants using this value
                await SoftDeleteVariantsUsingValueAsync(unprocessedValue.Id, cancellationToken);
            }
        }

        // Soft delete unprocessed options
        foreach (var unprocessedOption in existingOptionsDict.Values)
        {
            unprocessedOption.MarkAsDeleted();
            // CASCADE: Soft delete all values
            var valuesToDelete = existingValues.Where(v => v.ProductOptionTypeId == unprocessedOption.Id).ToList();
            foreach (var val in valuesToDelete)
            {
                val.MarkAsDeleted();
            }
            // CASCADE: Soft delete all variants
            await SoftDeleteVariantsUsingOptionAsync(unprocessedOption.Id, cancellationToken);
        }

        // ===== BƯỚC 6: Update Variants (Incremental) =====
        var existingVariants = await _productVariantRepository.GetByProductIdAsync(product.Id, cancellationToken);
        var existingVariantsDict = existingVariants.ToDictionary(v => v.Id);

        foreach (var variantDto in request.Variants)
        {
            ProductVariant variant;

            if (variantDto.Id.HasValue)
            {
                // Update existing variant
                if (existingVariantsDict.TryGetValue(variantDto.Id.Value, out var existingVariant))
                {
                    existingVariant.UpdateDetails(
                        variantDto.Sku ?? $"{request.Sku}-{Guid.NewGuid().ToString()[..8]}",
                        variantDto.Price,
                        variantDto.Stock,
                        variantDto.Weight ?? request.DefaultWeight ?? 0,
                        variantDto.Height ?? request.DefaultHeight ?? 0,
                        variantDto.Width ?? request.DefaultWidth ?? 0,
                        variantDto.Length ?? request.DefaultLength ?? 0
                    );
                    variant = existingVariant;
                    existingVariantsDict.Remove(variantDto.Id.Value);

                    // Update variant option values (delete old, create new)
                    var existingVOVs = await _productVariantOptionValueRepository.GetByVariantIdAsync(variant.Id, cancellationToken);
                    foreach (var vov in existingVOVs)
                    {
                        vov.MarkAsDeleted();
                    }

                    // Update variant medias (incremental)
                    if (variantDto.Medias != null && variantDto.Medias.Any())
                    {
                        var existingVariantMedias = await _productMediaRepository.GetByProductVariantIdAsync(variant.Id, cancellationToken);
                        var existingVariantMediasDict = existingVariantMedias.ToDictionary(m => m.Id);

                        foreach (var mediaDto in variantDto.Medias)
                        {
                            if (mediaDto.Id.HasValue)
                            {
                                // Update existing variant media
                                if (existingVariantMediasDict.TryGetValue(mediaDto.Id.Value, out var existingMedia))
                                {
                                    existingMedia.UpdateCoverStatus(mediaDto.IsCover);
                                    existingMedia.UpdateSortOrder(mediaDto.SortOrder);
                                    existingVariantMediasDict.Remove(mediaDto.Id.Value);
                                }
                            }
                            else
                            {
                                // Create new variant media
                                if (string.IsNullOrWhiteSpace(mediaDto.MediaUrl))
                                {
                                    return Result.Failure<ProductDetailDto>(ProductErrors.MediaNotFound("Variant media: MediaUrl is required"));
                                }

                                var mediaEntity = await _mediaRepository.GetByFileUrlAsync(mediaDto.MediaUrl, cancellationToken);
                                if (mediaEntity == null)
                                {
                                    return Result.Failure<ProductDetailDto>(ProductErrors.MediaNotFound($"Variant media: {mediaDto.MediaUrl}"));
                                }

                                var variantMedia = ProductMedia.CreateForVariant(
                                    product.Id,
                                    variant.Id,
                                    mediaEntity.Id,
                                    mediaDto.IsCover,
                                    mediaDto.SortOrder
                                );
                                await _productMediaRepository.AddAsync(variantMedia, cancellationToken);
                            }
                        }

                        // Soft delete unprocessed variant medias
                        foreach (var unprocessedMedia in existingVariantMediasDict.Values)
                        {
                            unprocessedMedia.MarkAsDeleted();
                        }
                    }
                }
                else
                {
                    return Result.Failure<ProductDetailDto>(ProductErrors.VariantNotFound(variantDto.Id.Value));
                }
            }
            else
            {
                // Create new variant
                variant = ProductVariant.Create(
                    product.Id,
                    variantDto.Sku ?? $"{request.Sku}-{Guid.NewGuid().ToString()[..8]}",
                    variantDto.Price,
                    variantDto.Stock,
                    variantDto.Weight ?? request.DefaultWeight ?? 0,
                    variantDto.Height ?? request.DefaultHeight ?? 0,
                    variantDto.Width ?? request.DefaultWidth ?? 0,
                    variantDto.Length ?? request.DefaultLength ?? 0
                );
                await _productVariantRepository.AddAsync(variant, cancellationToken);
            }

            // Create ProductVariantOptionValue links
            if (variantDto.OptionValues != null)
            {
                foreach (var optionValueString in variantDto.OptionValues)
                {
                    // Find option value by string
                    var optionValue = processedValues.Values
                        .SelectMany(v => v)
                        .FirstOrDefault(v => v.Value == optionValueString);

                    if (optionValue == null)
                    {
                        return Result.Failure<ProductDetailDto>(ProductErrors.InvalidOptionValue(optionValueString, ""));
                    }

                    var vov = ProductVariantOptionValue.Create(variant.Id, optionValue.Id);
                    await _productVariantOptionValueRepository.AddAsync(vov, cancellationToken);
                }
            }

            // Create variant medias
            if (variantDto.Medias != null && variantDto.Medias.Any())
            {
                foreach (var mediaDto in variantDto.Medias)
                {
                    if (string.IsNullOrWhiteSpace(mediaDto.MediaUrl))
                    {
                        return Result.Failure<ProductDetailDto>(ProductErrors.MediaNotFound("Variant media: MediaUrl is required"));
                    }

                    var mediaEntity = await _mediaRepository.GetByFileUrlAsync(mediaDto.MediaUrl, cancellationToken);
                    if (mediaEntity == null)
                    {
                        return Result.Failure<ProductDetailDto>(ProductErrors.MediaNotFound($"Variant media: {mediaDto.MediaUrl}"));
                    }

                    var variantMedia = ProductMedia.CreateForVariant(
                        product.Id,
                        variant.Id,
                        mediaEntity.Id,
                        mediaDto.IsCover,
                        mediaDto.SortOrder
                    );
                    await _productMediaRepository.AddAsync(variantMedia, cancellationToken);
                }
            }
        }

        // Soft delete unprocessed variants
        foreach (var unprocessedVariant in existingVariantsDict.Values)
        {
            unprocessedVariant.MarkAsDeleted();
            // CASCADE: Soft delete VOVs and medias
            var vovsToDelete = await _productVariantOptionValueRepository.GetByVariantIdAsync(unprocessedVariant.Id, cancellationToken);
            foreach (var vov in vovsToDelete)
            {
                vov.MarkAsDeleted();
            }
        }

        // ===== BƯỚC 7: Save Changes =====
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ===== BƯỚC 8: Return Updated Product Details =====
        // TODO: Query product details using Dapper similar to GetProductsQueryHandler
        // For now, return a basic DTO
        return Result.Success(ProductDetailDto.CreateClean(
            id: product.Id,
            shopId: product.ShopId,
            categoryId: product.CategoryId,
            name: product.Name,
            description: product.Description,
            slug: product.Slug,
            sku: product.Sku,
            status: product.Status,
            createdAtUtc: product.CreatedAtUtc,
            updatedAtUtc: product.UpdatedAtUtc,
            medias: new List<ProductMediaDto>(),
            hasVariants: request.Options.Count > 0,
            variantCount: request.Variants.Count
        ));
    }

    private async Task SoftDeleteVariantsUsingValueAsync(Guid optionValueId, CancellationToken cancellationToken)
    {
        var vovsUsingValue = await _productVariantOptionValueRepository.GetByOptionValueIdAsync(optionValueId, cancellationToken);
        var variantIdsToDelete = vovsUsingValue.Select(vov => vov.VariantId).ToList();

        foreach (var variantId in variantIdsToDelete)
        {
            var variant = await _productVariantRepository.GetByIdAsync(variantId, cancellationToken);
            variant?.MarkAsDeleted();
        }
    }

    private async Task SoftDeleteVariantsUsingOptionAsync(Guid optionTypeId, CancellationToken cancellationToken)
    {
        var valuesToDelete = await _productOptionValueRepository.GetByProductOptionTypeIdAsync(optionTypeId, cancellationToken);

        foreach (var value in valuesToDelete)
        {
            await SoftDeleteVariantsUsingValueAsync(value.Id, cancellationToken);
        }
    }
}

