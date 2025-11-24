using System.Data.Common;
using System.Globalization;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Application.Contracts.Categories;

namespace ECommerceBackend.Application.Categories.SearchCategory;

/// PBNMinh - 08/09/2025
public sealed class SearchCategoryQueryHandler : IQueryHandler<SearchCategoryQuery, List<CategoryTreeDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SearchCategoryQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<List<CategoryTreeDto>>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                icon_url AS IconUrl,
                status AS Status,
                parent_id AS ParentId,
                depth AS Depth,
                created_at_utc AS CreatedAtUtc,
                updated_at_utc AS UpdatedAtUtc
            FROM "ecommerce-domain".categories
            WHERE status = 'ACTIVE'
        """;

        IEnumerable<CategoryTreeDto> flatList = await connection.QueryAsync<CategoryTreeDto>(sql);
        var categories = flatList.ToList();


        var lookup = categories.ToDictionary(c => c.Id, c => c);
        List<CategoryTreeDto> roots = new();

        foreach (CategoryTreeDto category in categories)
        {
            if (category.ParentId == null || category.ParentId == Guid.Empty)
            {
                roots.Add(category);
            }
            else if (lookup.TryGetValue(category.ParentId.Value, out CategoryTreeDto parent))
            {
                parent.Children.Add(category);
            }
        }

        string queryText = request.QueryText.Trim();

        var matchedNodes = categories
            .Where(c => c.Name.Contains(queryText, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matchedNodes.Any())
        {
            return Result.Success(new List<CategoryTreeDto>());
        }

        HashSet<Guid> includedIds = new();
        foreach (CategoryTreeDto match in matchedNodes)
        {
            CollectDescendants(match, includedIds);
        }

        List<CategoryTreeDto> filteredTree = roots
            .Select(r => FilterTree(r, includedIds))
            .Where(r => r is not null)
            .ToList()!;

        return Result.Success(filteredTree);
    }

    private static void CollectDescendants(CategoryTreeDto node, HashSet<Guid> includedIds)
    {
        if (includedIds.Contains(node.Id))
        { return; }

        includedIds.Add(node.Id);

        foreach (CategoryTreeDto child in node.Children)
        {
            CollectDescendants(child, includedIds);
        }
    }

    private static CategoryTreeDto? FilterTree(CategoryTreeDto node, HashSet<Guid> includedIds)
    {
        if (!includedIds.Contains(node.Id))
        {
            List<CategoryTreeDto> filteredChildren = node.Children
                .Select(c => FilterTree(c, includedIds))
                .Where(c => c is not null)
                .ToList()!;

            if (filteredChildren.Count == 0)
            { return null; }

            node.Children = filteredChildren;
            return node;
        }

        node.Children = node.Children
            .Select(c => FilterTree(c, includedIds))
            .Where(c => c is not null)
            .ToList()!;

        return node;
    }
}
