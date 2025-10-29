using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Products.Queries.GetProductsByCategory;

public sealed class GetProductsByCategoryQueryHandler : IQueryHandler<GetProductsByCategoryQuery, PaginationResult<ProductDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetProductsByCategoryQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<PaginationResult<ProductDto>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        // Build WHERE conditions
        var whereConditions = new List<string>
        {
            "p.status = @ProductStatus",
            "p.category_id = @CategoryId"
        };
        var parameters = new Dictionary<string, object>
        {
            ["ProductStatus"] = "Active",
            ["CategoryId"] = request.CategoryId
        };

        // Text search with fulltext
        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            whereConditions.Add("(p.name ILIKE @SearchText OR p.description ILIKE @SearchText)");
            parameters["SearchText"] = $"%{request.Q}%";
        }

        // Shop filter
        if (request.ShopId.HasValue)
        {
            whereConditions.Add("p.shop_id = @ShopId");
            parameters["ShopId"] = request.ShopId.Value;
        }

        // Price range filter
        if (request.MinPrice.HasValue)
        {
            whereConditions.Add("pv.price >= @MinPrice");
            parameters["MinPrice"] = request.MinPrice.Value;
        }

        if (request.MaxPrice.HasValue)
        {
            whereConditions.Add("pv.price <= @MaxPrice");
            parameters["MaxPrice"] = request.MaxPrice.Value;
        }

        // Pickup location filter
        if (!string.IsNullOrWhiteSpace(request.PickupProvince))
        {
            whereConditions.Add("EXISTS (SELECT 1 FROM \"ecommerce-domain\".addresses a WHERE a.user_id = s.owner_id AND a.is_pick_up_address = true AND a.province = @PickupProvince)");
            parameters["PickupProvince"] = request.PickupProvince;
        }

        if (!string.IsNullOrWhiteSpace(request.PickupDistrict))
        {
            whereConditions.Add("EXISTS (SELECT 1 FROM \"ecommerce-domain\".addresses a WHERE a.user_id = s.owner_id AND a.is_pick_up_address = true AND a.district = @PickupDistrict)");
            parameters["PickupDistrict"] = request.PickupDistrict;
        }

        // Promotion filter (tạm thời luôn false)
        if (request.HasPromotion == true)
        {
            whereConditions.Add("false");
        }

        string whereClause = string.Join(" AND ", whereConditions);

        // Build ORDER BY
        string orderBy = request.SortBy switch
        {
            ProductSortBy.Newest => "p.created_at_utc DESC",
            ProductSortBy.PriceAsc => "min_price ASC",
            ProductSortBy.PriceDesc => "min_price DESC",
            ProductSortBy.Relevance when !string.IsNullOrWhiteSpace(request.Q) =>
                "CASE WHEN p.name ILIKE @SearchText THEN 1 ELSE 2 END, p.created_at_utc DESC",
            _ => "p.created_at_utc DESC"
        };

        // Main query
        string mainSql = $"""
            SELECT 
                p.id AS Id,
                p.shop_id AS ShopId,
                p.category_id AS CategoryId,
                p.name AS Name,
                p.description AS Description,
                p.slug AS Slug,
                p.status AS Status,
                p.created_at_utc AS CreatedAtUtc,
                p.updated_at_utc AS UpdatedAtUtc,
                MIN(pv.price) AS MinPrice,
                MAX(pv.price) AS MaxPrice,
                CAST(SUM(pv.stock) AS BIGINT) AS TotalStock,
                CASE WHEN COUNT(pv.id) > 1 THEN true ELSE false END AS HasVariants
            FROM "ecommerce-domain".products p
            INNER JOIN "ecommerce-domain".shops s ON p.shop_id = s.id
            LEFT JOIN "ecommerce-domain".product_variants pv ON p.id = pv.product_id AND pv.status = @VariantStatus
            WHERE {whereClause}
            GROUP BY p.id, p.shop_id, p.category_id, p.name, p.description, p.slug, p.status, p.created_at_utc, p.updated_at_utc
            ORDER BY {orderBy}
            LIMIT @PageSize OFFSET (@Page - 1) * @PageSize
            """;

        // Count query
        string countSql = $"""
            SELECT COUNT(DISTINCT p.id)
            FROM "ecommerce-domain".products p
            INNER JOIN "ecommerce-domain".shops s ON p.shop_id = s.id
            LEFT JOIN "ecommerce-domain".product_variants pv ON p.id = pv.product_id AND pv.status = @VariantStatus
            WHERE {whereClause}
            """;

        // Add pagination parameters
        parameters["Page"] = request.Page;
        parameters["PageSize"] = request.PageSize;
        parameters["VariantStatus"] = "Active";

        // Execute queries
        IEnumerable<ProductDto> products = await connection.QueryAsync<ProductDto>(mainSql, parameters);
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var paginationResult = PaginationResult<ProductDto>.CreateAsync(
            products.ToList(),
            request.Page,
            request.PageSize,
            totalCount
        );

        return Result.Success(paginationResult);
    }
}
