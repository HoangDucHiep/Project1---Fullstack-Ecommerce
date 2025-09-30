using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.SearchCategory;

public sealed class SearchCategoryQueryHandler : IQueryHandler<SearchCategoryQuery, List<CategoryResponse>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SearchCategoryQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<List<CategoryResponse>>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
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
            FROM categories
            WHERE status = 'ACTIVE' AND name ILIKE @QueryText
        """;

        // Dùng explicit type
        IEnumerable<CategoryResponse> categories = await connection.QueryAsync<CategoryResponse>(
            sql,
            new { QueryText = $"%{request.QueryText}%" }
        );
        var result = categories.ToList();


        // Trả Success luôn, kể cả khi rỗng
        return Result.Success(result);
    }
}
