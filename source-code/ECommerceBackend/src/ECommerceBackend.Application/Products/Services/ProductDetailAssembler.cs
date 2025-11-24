using ECommerceBackend.Application.Contracts.Media;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Domain.Medias;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Products.Services;

public sealed class ProductDetailAssembler
{
    private readonly IProductOptionValueRepository _productOptionValueRepository;
    private readonly IProductVariantOptionValueRepository _productVariantOptionValueRepository;
    private readonly IProductMediaRepository _productMediaRepository;
    private readonly IMediaRepository _mediaRepository;

    public ProductDetailAssembler(
        IProductOptionValueRepository productOptionValueRepository,
        IProductVariantOptionValueRepository productVariantOptionValueRepository,
        IProductMediaRepository productMediaRepository,
        IMediaRepository mediaRepository)
    {
        _productOptionValueRepository = productOptionValueRepository;
        _productVariantOptionValueRepository = productVariantOptionValueRepository;
        _productMediaRepository = productMediaRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task<List<ProductMediaDto>> BuildMediaDtos(List<ProductMedia> productMedias, CancellationToken cancellationToken)
    {
        var mediaDtos = new List<ProductMediaDto>();

        foreach (ProductMedia productMedia in productMedias)
        {
            Media? media = await _mediaRepository.GetByIdAsync(productMedia.MediaId, cancellationToken);
            if (media != null)
            {
                mediaDtos.Add(new ProductMediaDto(
                    productMedia.Id,
                    productMedia.ProductId,
                    productMedia.ProductVariantId,
                    productMedia.MediaId,
                    productMedia.IsCover,
                    productMedia.SortOrder,
                    productMedia.CreatedAtUtc,
                    MediaDto.FromEntity(media)
                ));
            }
        }

        return mediaDtos;
    }

    public async Task<List<ProductMediaDto>> BuildVariantMediaDtos(List<ProductMedia> variantMedias, CancellationToken cancellationToken)
    {
        // Reuse same logic
        return await BuildMediaDtos(variantMedias, cancellationToken);
    }

    public async Task<List<ProductOptionDto>> BuildOptionDtos(List<ProductOptionType> productOptions, CancellationToken cancellationToken)
    {
        var optionDtos = new List<ProductOptionDto>();

        foreach (ProductOptionType option in productOptions)
        {
            List<ProductOptionValue> optionValues = await _productOptionValueRepository.GetByProductOptionTypeIdAsync(option.Id, cancellationToken);
            optionDtos.Add(new ProductOptionDto(option.Name, optionValues.Select(v => v.Value).ToList()));
        }

        return optionDtos;
    }

    public async Task<List<ProductVariantDetailDto>> BuildVariantDtos(List<ProductVariant> productVariants, CancellationToken cancellationToken)
    {
        var variantDtos = new List<ProductVariantDetailDto>();

        foreach (ProductVariant variant in productVariants)
        {
            List<ProductVariantOptionValue> variantOptionValues =
                await _productVariantOptionValueRepository.GetByVariantIdAsync(variant.Id, cancellationToken);

            var optionValues = new List<string>();
            foreach (ProductVariantOptionValue vov in variantOptionValues)
            {
                ProductOptionValue? optionValue = await _productOptionValueRepository.GetByIdAsync(vov.OptionValueId, cancellationToken);
                if (optionValue != null)
                {
                    optionValues.Add(optionValue.Value);
                }
            }

            List<ProductMedia> variantMedias = await _productMediaRepository.GetByProductVariantIdAsync(variant.Id, cancellationToken);
            List<ProductMediaDto> variantMediaDtos = await BuildVariantMediaDtos(variantMedias, cancellationToken);

            variantDtos.Add(new ProductVariantDetailDto(
                variant.Id,
                optionValues,
                variant.Price,
                variant.Stock,
                variant.Sku,
                variant.Status,
                (decimal)variant.Weight,
                (decimal)variant.Height,
                (decimal)variant.Width,
                (decimal)variant.Length,
                variantMediaDtos,
                variant.CreatedAtUtc
            ));
        }

        return variantDtos;
    }

    // Dùng khi Build chi tiết cần override dữ liệu cho simple product (từ request Create)
    public sealed record SimpleOverrides(
        decimal? Price,
        int? Stock,
        decimal? Weight,
        decimal? Height,
        decimal? Width,
        decimal? Length
    );

    public ProductDetailDto BuildProductDetail(
        Product product,
        List<ProductMediaDto> mediaDtos,
        List<ProductOptionType> productOptions,
        List<ProductVariant> productVariants,
        List<ProductOptionDto>? optionDtos,                       // null nếu simple
        List<ProductVariantDetailDto>? variantDtos,               // null nếu simple
        SimpleOverrides? simpleOverrides                          // null nếu có variants
    )
    {
        bool hasVariants = productOptions.Any();

        if (hasVariants)
        {
            decimal minPrice = productVariants.Min(v => v.Price);
            decimal maxPrice = productVariants.Max(v => v.Price);
            int totalStock = productVariants.Sum(v => v.Stock);

            return ProductDetailDto.CreateClean(
                product.Id,
                product.ShopId,
                product.CategoryId,
                product.Name,
                product.Description,
                product.Slug,
                product.Sku,
                product.Status,
                product.CreatedAtUtc,
                product.UpdatedAtUtc,
                mediaDtos,
                hasVariants: true,
                variantCount: productVariants.Count,
                options: optionDtos,
                variants: variantDtos,
                minPrice: minPrice,
                maxPrice: maxPrice,
                totalStock: totalStock
            );
        }
        else
        {
            // Nếu không có override (trường hợp Query), lấy từ ghost variant
            ProductVariant? defaultVariant = productVariants.FirstOrDefault();

            decimal? price = simpleOverrides?.Price ?? defaultVariant?.Price;
            int? stock = simpleOverrides?.Stock ?? defaultVariant?.Stock;
            decimal? weight = simpleOverrides?.Weight ?? (defaultVariant != null ? (decimal?)defaultVariant.Weight : null);
            decimal? height = simpleOverrides?.Height ?? (defaultVariant != null ? (decimal?)defaultVariant.Height : null);
            decimal? width = simpleOverrides?.Width ?? (defaultVariant != null ? (decimal?)defaultVariant.Width : null);
            decimal? length = simpleOverrides?.Length ?? (defaultVariant != null ? (decimal?)defaultVariant.Length : null);

            return ProductDetailDto.CreateClean(
                product.Id,
                product.ShopId,
                product.CategoryId,
                product.Name,
                product.Description,
                product.Slug,
                product.Sku,
                product.Status,
                product.CreatedAtUtc,
                product.UpdatedAtUtc,
                mediaDtos,
                hasVariants: false,
                variantCount: 0,
                price: price,
                stock: stock,
                weight: weight,
                height: height,
                width: width,
                length: length
            );
        }
    }
}
