using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Categories.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCategoryByIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """           
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
            WHERE id = @CategoryId      
            """;


        CategoryResponse? category =
            await connection.QueryFirstOrDefaultAsync<CategoryResponse>(sql, new { request.CategoryId });

        if (category is null)
        {
            return Result.Failure<CategoryResponse>(UserErrors.NotFound(request.CategoryId));
        }

        return category;

    }
}
