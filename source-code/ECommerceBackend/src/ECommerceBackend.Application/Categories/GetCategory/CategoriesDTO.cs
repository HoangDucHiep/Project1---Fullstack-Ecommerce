using System;
using System.Collections.Generic;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories;

public sealed class CategoriesDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } // Đổi từ private set -> public set
    public string IconUrl { get; set; } // Đổi từ private set -> public set
    public CategoryStatus Status { get; set; } // Đổi từ private set -> public set
    public Guid? ParentId { get; set; } // Đổi từ private set -> public set
    public int Lft { get; set; } // Đổi từ private set -> public set
    public int Rgt { get; set; } // Đổi từ private set -> public set
    public int Depth { get; set; } // Đổi từ private set -> public set

    public DateTimeOffset CreatedAtUtc { get; set; } // Đổi từ private set -> public set
    public DateTimeOffset UpdatedAtUtc { get; set; } // Đổi từ private set -> public set

    public List<CategoriesDTO> Children { get; set; } = new List<CategoriesDTO>();
}
