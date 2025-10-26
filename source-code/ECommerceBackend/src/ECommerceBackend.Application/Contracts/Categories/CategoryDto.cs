using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBackend.Application.Contracts.Categories;

/// PBNMinh- 08/09/2025
public sealed record class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }
    public string Status { get; set; }
    public Guid? ParentId { get; set; }
    public int Depth { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
