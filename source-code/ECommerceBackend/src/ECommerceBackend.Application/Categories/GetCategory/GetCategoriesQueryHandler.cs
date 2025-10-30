using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Categories.GetCategories;

internal sealed class GetCategoriesHandler : IQueryHandler<GetCategoriesQuery, List<GetCategoriesTreeDTO>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCategoriesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<List<GetCategoriesTreeDTO>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection dbConnection = await _dbConnectionFactory.OpenConnectionAsync();

        const string sql = $"""
            SELECT 
                id as Id,
                name as Name,
                icon_url as IconUrl,
                status as Status,
                parent_id as ParentId,
                depth as Depth,
                created_at_utc as CreatedAtUtc,
                updated_at_utc as UpdatedAtUtc
             FROM "ecommerce-domain".categories
            """;

        // explicit type thay cho var ở đây
        IEnumerable<GetCategoriesTreeDTO> flatList = await dbConnection.QueryAsync<GetCategoriesTreeDTO>(sql);
        var categories = flatList.ToList();
        var lookup = categories.ToDictionary(c => c.Id, c => c);
        var roots = new List<GetCategoriesTreeDTO>();

        // explicit type thay cho var ở đây
        foreach (GetCategoriesTreeDTO category in categories)
        {
            if (category.ParentId == null || category.ParentId == Guid.Empty)
            {
                roots.Add(category);
            }
            else if (lookup.TryGetValue(category.ParentId.Value, out GetCategoriesTreeDTO parent))
            {
                parent.Children.Add(category);
            }
        }

        return Result.Success(roots);
    }
}
