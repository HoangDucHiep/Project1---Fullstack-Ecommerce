using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.SearchCategory;
public sealed record CategoryResponse
{
    public string Name { get; private set; }
    public string IconUrl { get; private set; }
    public CategoryStatus Status { get; private set; }
    public Guid? ParentId { get; private set; }
    public int Lft { get; private set; }
    public int Rgt { get; private set; }
    public int Depth { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
}
