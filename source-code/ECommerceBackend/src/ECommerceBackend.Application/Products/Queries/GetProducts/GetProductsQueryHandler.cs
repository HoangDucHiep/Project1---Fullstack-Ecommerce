#pragma warning disable IDE0008
using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PaginationResult<ProductListItemDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetProductsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<PaginationResult<ProductListItemDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        var filter = request.Filter;
        var whereConditions = new List<string> { "p.status = 'Active'" };
        var parameters = new DynamicParameters();

        // Build WHERE conditions
        if (filter.CategoryId.HasValue)
        {
            whereConditions.Add("p.category_id = @CategoryId");
            parameters.Add("CategoryId", filter.CategoryId.Value);
        }

        if (filter.ShopId.HasValue)
        {
            whereConditions.Add("p.shop_id = @ShopId");
            parameters.Add("ShopId", filter.ShopId.Value);
        }

        if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
        {
            if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue)
            {
                whereConditions.Add("(pv.price BETWEEN @MinPrice AND @MaxPrice)");
                parameters.Add("MinPrice", filter.MinPrice.Value);
                parameters.Add("MaxPrice", filter.MaxPrice.Value);
            }
            else if (filter.MinPrice.HasValue)
            {
                whereConditions.Add("pv.price >= @MinPrice");
                parameters.Add("MinPrice", filter.MinPrice.Value);
            }
            else if (filter.MaxPrice.HasValue)
            {
                whereConditions.Add("pv.price <= @MaxPrice");
                parameters.Add("MaxPrice", filter.MaxPrice.Value);
            }
        }

        // Build ORDER BY clause
        string orderBy = filter.SortBy switch
        {
            ProductSortBy.Newest => "created_at_utc DESC",
            ProductSortBy.PriceLowToHigh => "min_price ASC",
            ProductSortBy.PriceHighToLow => "max_price DESC",
            _ => "created_at_utc DESC" // Default to newest
        };

        // Calculate pagination
        int offset = (filter.Page - 1) * filter.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", filter.PageSize);

        // Main query with aggregated data
        string sql = $"""
            WITH product_aggregates AS (
                SELECT 
                    p.id,
                    p.shop_id,
                    p.category_id,
                    p.name,
                    p.slug,
                    p.created_at_utc,
                    MIN(pv.price) as min_price,
                    MAX(pv.price) as max_price,
                    -- Get cover image
                    (SELECT pm.media_url 
                     FROM "ecommerce-domain".product_medias pm 
                     WHERE pm.product_id = p.id 
                       AND pm.product_variant_id IS NULL 
                       AND pm.is_cover = true 
                     LIMIT 1) as cover_image_url,
                    -- Discount info (placeholder for now)
                    false as has_discount,
                    null as discount_percentage
                FROM "ecommerce-domain".products p
                INNER JOIN "ecommerce-domain".product_variants pv ON p.id = pv.product_id
                WHERE {string.Join(" AND ", whereConditions)}
                GROUP BY p.id, p.shop_id, p.category_id, p.name, p.slug, p.created_at_utc
            )
            SELECT 
                id as Id,
                shop_id as ShopId,
                category_id as CategoryId,
                name as Name,
                slug as Slug,
                min_price as MinPrice,
                max_price as MaxPrice,
                COALESCE(cover_image_url, '') as CoverImageUrl,
                has_discount as HasDiscount,
                discount_percentage as DiscountPercentage,
                created_at_utc as CreatedAtUtc
            FROM product_aggregates
            ORDER BY {orderBy}
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
        """;

        // Count query
        string countSql = $"""
            SELECT COUNT(DISTINCT p.id)
            FROM "ecommerce-domain".products p
            INNER JOIN "ecommerce-domain".product_variants pv ON p.id = pv.product_id
            WHERE {string.Join(" AND ", whereConditions)}
        """;

        // Execute queries
        var productResults = await connection.QueryAsync<ProductListResult>(sql, parameters);
        var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

        // Map to DTOs
        var products = productResults.Select(p => new ProductListItemDto(
            Id: p.Id,
            Name: p.Name,
            Slug: p.Slug,
            MinPrice: p.MinPrice,
            MaxPrice: p.MaxPrice,
            CoverImageUrl: p.CoverImageUrl,
            ShopId: p.ShopId,
            CategoryId: p.CategoryId,
            HasDiscount: p.HasDiscount,
            DiscountPercentage: p.DiscountPercentage,
            CreatedAtUtc: p.CreatedAtUtc
        ));

        var result = PaginationResult<ProductListItemDto>.CreateAsync(products, filter.Page, filter.PageSize, totalCount);
        return Result.Success(result);
    }
}

// Internal DTO for Dapper mapping
internal sealed class ProductListResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public Guid ShopId { get; set; }
    public Guid CategoryId { get; set; }
    public bool HasDiscount { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
