using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.SearchCategory;

/// PBNMinh- 08/09/2025
public sealed class SearchCategoryQueryHandler : IQueryHandler<SearchCategoryQuery, List<CategoryDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SearchCategoryQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<List<CategoryDto>>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                icon_url AS IconUrl,
                status AS Status,
                parent_id AS ParentId,
                lft AS Lft,
                rgt AS Rgt,
                depth AS Depth,
                created_at_utc AS CreatedAtUtc,
                updated_at_utc AS UpdatedAtUtc
            FROM "ecommerce-domain".categories
            WHERE status = 'ACTIVE' AND name ILIKE @QueryText
        """;


        IEnumerable<CategoryDto> categories = await connection.QueryAsync<CategoryDto>(
            sql,
            new { QueryText = $"%{request.QueryText}%" }
        );
        var result = categories.ToList();

        return Result.Success(result);


    }
}
