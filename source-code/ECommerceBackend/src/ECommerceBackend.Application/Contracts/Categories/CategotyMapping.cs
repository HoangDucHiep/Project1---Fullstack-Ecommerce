using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Contracts.Categories;

/// PBNMinh- 08/09/2025
public static class CategoryMappings
{
    public static CategoryDto ToCategoryDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            IconUrl = category.IconUrl,
            ParentId = category.ParentId,
            Lft = category.Lft,
            Rgt = category.Rgt,
            Depth = category.Depth,
            CreatedAtUtc = category.CreatedAtUtc,
            UpdatedAtUtc = category.UpdatedAtUtc
        };
    }

    //public static Category ToCategoryEntity(this CategoryDto dto)
    //{
    //    return Category.Create(
    //        dto.Name,
    //        dto.IconUrl,
    //        dto.ParentId,
    //        dto.Lft,
    //        dto.Rgt,
    //        dto.Depth
    //    );
    //}
}

