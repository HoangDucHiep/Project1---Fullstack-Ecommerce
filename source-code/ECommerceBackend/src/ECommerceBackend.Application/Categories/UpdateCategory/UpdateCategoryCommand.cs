using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.UpdateCategory;
public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? IconUrl,
    CategoryStatus Status,
    Guid? NewParentId
) : ICommand<CategoryDto>;

