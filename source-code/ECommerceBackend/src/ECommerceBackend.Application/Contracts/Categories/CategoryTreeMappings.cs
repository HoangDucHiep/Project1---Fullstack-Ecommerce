using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Contracts.Categories;
public static class CategoryTreeMappings
{
    public static CategoryTreeDto ToCategoryTreeDto(this Category category)
    {
        return new CategoryTreeDto
        {
            Id = category.Id,
            Name = category.Name,
            IconUrl = category.IconUrl,
            ParentId = category.ParentId,
            Depth = category.Depth,
            Status = category.Status.ToString()
        };
    }

    public static CategoryTreeDto ToCategoryTree(this List<Category> categories, Guid rootId)
    {
        var lookup = categories.ToDictionary(c => c.Id, c => c.ToCategoryTreeDto());
        CategoryTreeDto? root = null;

        foreach (CategoryTreeDto cat in lookup.Values)
        {
            if (cat.ParentId is not null && lookup.TryGetValue(cat.ParentId.Value, out CategoryTreeDto? parent))
            {
                parent.Children ??= new List<CategoryTreeDto>();
                parent.Children.Add(cat);
            }

            if (cat.Id == rootId)
            { root = cat; }
        }

        return root!;
    }
}

