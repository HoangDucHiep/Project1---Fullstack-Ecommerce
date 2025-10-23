#pragma warning disable IDE0008
using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Products.Queries.GetProduct;

internal sealed class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDetailDto>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetProductQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<ProductDetailDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        // 1. Get product basic info (only Active products for public view)
        const string productSql = """
            SELECT 
                id AS Id,
                shop_id AS ShopId,
                category_id AS CategoryId,
                name AS Name,
                slug AS Slug,
                description AS Description,
                status AS Status,
                created_at_utc AS CreatedAtUtc,
                updated_at_utc AS UpdatedAtUtc
            FROM "ecommerce-domain".products 
            WHERE id = @ProductId AND status = 'Active'
        """;

        var product = await connection.QuerySingleOrDefaultAsync<ProductBasicDto>(productSql, new { request.ProductId });
        if (product == null)
        {
            return Result.Failure<ProductDetailDto>(ProductErrors.NotFound(request.ProductId));
        }

        // 2. Get product media (product-level only)
        const string mediaSql = """
            SELECT 
                media_url AS MediaUrl,
                media_type AS MediaType,
                sort_order AS SortOrder,
                is_cover AS IsCover
            FROM "ecommerce-domain".product_medias 
            WHERE product_id = @ProductId AND product_variant_id IS NULL
            ORDER BY sort_order
        """;

        var productMedia = await connection.QueryAsync<ProductMediaDto>(mediaSql, new { request.ProductId });

        // 3. Get product options with values
        const string optionsSql = """
            SELECT 
                pot.id AS OptionTypeId,
                pot.name AS OptionName,
                pov.value AS OptionValue,
                pot.created_at_utc AS OptionCreatedAt,
                pov.created_at_utc AS ValueCreatedAt
            FROM "ecommerce-domain".product_option_types pot
            LEFT JOIN "ecommerce-domain".product_option_values pov ON pot.id = pov.product_option_type_id
            WHERE pot.product_id = @ProductId
            ORDER BY pot.created_at_utc, pov.created_at_utc
        """;

        var optionResults = await connection.QueryAsync<OptionValueResult>(optionsSql, new { request.ProductId });
        
        var optionDtos = optionResults
            .GroupBy(o => new { o.OptionTypeId, o.OptionName })
            .Select(g => new ProductOptionDto(
                Name: g.Key.OptionName,
                Values: g.Where(x => !string.IsNullOrEmpty(x.OptionValue))
                        .Select(x => x.OptionValue!)
                        .ToList()
            ))
            .ToList();

        // 4. Get variants with their option values and media
        const string variantsSql = """
            SELECT 
                pv.id AS VariantId,
                pv.price AS Price,
                pv.stock AS Stock,
                pv.sku AS Sku,
                pv.weight AS Weight,
                pv.height AS Height,
                pv.width AS Width,
                pv.length AS Length,
                pv.created_at_utc AS VariantCreatedAt,
                
                -- Option values
                pov.value AS OptionValue,
                pot.name AS OptionName,
                pot.created_at_utc AS OptionTypeCreatedAt,
                
                -- Variant media
                pm.media_url AS MediaUrl,
                pm.media_type AS MediaType,
                pm.sort_order AS SortOrder,
                pm.is_cover AS IsCover
            FROM "ecommerce-domain".product_variants pv
            LEFT JOIN "ecommerce-domain".product_variant_option_values pvov ON pv.id = pvov.variant_id
            LEFT JOIN "ecommerce-domain".product_option_values pov ON pvov.option_value_id = pov.id
            LEFT JOIN "ecommerce-domain".product_option_types pot ON pov.product_option_type_id = pot.id
            LEFT JOIN "ecommerce-domain".product_medias pm ON pv.id = pm.product_variant_id
            WHERE pv.product_id = @ProductId AND pv.stock > 0
            ORDER BY pv.created_at_utc, pot.created_at_utc, pm.sort_order
        """;

        var variantResults = await connection.QueryAsync<VariantResult>(variantsSql, new { request.ProductId });

        var variantDetailDtos = variantResults
            .GroupBy(v => new { 
                v.VariantId, v.Price, v.Stock, v.Sku, v.Weight, v.Height, v.Width, v.Length 
            })
            .Select(g =>
            {
                // Get option values in correct order
                var optionValues = g.Where(x => !string.IsNullOrEmpty(x.OptionValue))
                                   .GroupBy(x => x.OptionName)
                                   .OrderBy(x => x.First().OptionTypeCreatedAt)
                                   .Select(x => x.First().OptionValue!)
                                   .ToList();

                // Get variant media
                var variantMedia = g.Where(x => !string.IsNullOrEmpty(x.MediaUrl))
                                   .Select(x => new ProductMediaDto(
                                       MediaUrl: x.MediaUrl!,
                                       MediaType: x.MediaType!,
                                       SortOrder: x.SortOrder ?? 0,
                                       IsCover: x.IsCover ?? false
                                   ))
                                   .DistinctBy(x => x.MediaUrl)
                                   .OrderBy(x => x.SortOrder)
                                   .ToList();

                return new ProductVariantDetailDto(
                    Id: g.Key.VariantId,
                    OptionValues: optionValues,
                    Price: g.Key.Price,
                    Stock: g.Key.Stock,
                    Sku: g.Key.Sku,
                    Weight: g.Key.Weight,
                    Height: g.Key.Height,
                    Width: g.Key.Width,
                    Length: g.Key.Length,
                    VariantMedia: variantMedia.Any() ? variantMedia : null
                );
            })
            .ToList();

        // 5. Create result DTO
        var productDetailDto = new ProductDetailDto(
            Id: product.Id,
            ShopId: product.ShopId,
            CategoryId: product.CategoryId,
            Name: product.Name,
            Slug: product.Slug,
            Description: product.Description,
            Status: Enum.Parse<ProductStatus>(product.Status),
            Media: productMedia.ToList(),
            Options: optionDtos,
            Variants: variantDetailDtos,
            CreatedAtUtc: product.CreatedAtUtc,
            UpdatedAtUtc: product.UpdatedAtUtc
        );

        return Result.Success(productDetailDto);
    }
}

// Internal DTOs for Dapper mapping
internal sealed class ProductBasicDto
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Changed to string
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
}

internal sealed class OptionValueResult
{
    public Guid OptionTypeId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public string? OptionValue { get; set; }
    public DateTimeOffset OptionCreatedAt { get; set; }
    public DateTimeOffset? ValueCreatedAt { get; set; }
}

internal sealed class VariantResult
{
    public Guid VariantId { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Sku { get; set; } = string.Empty;
    public double Weight { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Length { get; set; }
    public DateTimeOffset VariantCreatedAt { get; set; }
    public string? OptionValue { get; set; }
    public string? OptionName { get; set; }
    public DateTimeOffset? OptionTypeCreatedAt { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsCover { get; set; }
}
