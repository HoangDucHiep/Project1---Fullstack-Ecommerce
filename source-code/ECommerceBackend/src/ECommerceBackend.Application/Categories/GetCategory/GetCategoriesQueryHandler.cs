using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Application.Categories; // CategoriesDTO

namespace ECommerceBackend.Application.Categories.GetCategories;

internal sealed class GetCategoriesHandler : IQueryHandler<GetCategoriesQuery, List<CategoriesDTO>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCategoriesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<List<CategoriesDTO>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection dbConnection = await _dbConnectionFactory.OpenConnectionAsync();

        const string sql = @"
    SELECT 
        id as Id,
        name as Name,
        icon_url as IconUrl,
        status as Status,
        parent_id as ParentId,
        lft as Lft,
        rgt as Rgt,
        depth as Depth,
        created_at_utc as CreatedAtUtc,
        updated_at_utc as UpdatedAtUtc
    FROM categories";

        // explicit type thay cho var ở đây
        IEnumerable<CategoriesDTO> flatList = await dbConnection.QueryAsync<CategoriesDTO>(sql);
        var categories = flatList.ToList();
        var lookup = categories.ToDictionary(c => c.Id, c => c);
        var roots = new List<CategoriesDTO>();

        // explicit type thay cho var ở đây
        foreach (CategoriesDTO category in categories)
        {
            if (category.ParentId == null || category.ParentId == Guid.Empty)
            {
                roots.Add(category);
            }
            else if (lookup.TryGetValue(category.ParentId.Value, out CategoriesDTO parent))
            {
                parent.Children.Add(category);
            }
        }

        return Result.Success(roots);
    }
}
