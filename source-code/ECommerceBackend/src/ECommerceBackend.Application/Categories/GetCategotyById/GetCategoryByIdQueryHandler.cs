using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.GetCategotyByID;
public sealed class GetCategoryByIdQueryHandler: IQueryHandler<GetCategoryByIdQuery, object>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<object>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure<object>(CategoryErrors.NotFound(request.CategoryId));
        }

        if (!request.IncludeChildren)
        {
            return Result.Success<object>(category.ToCategoryDto());
        }

        List<Category> allCategories = await _categoryRepository.GetAllAsync(cancellationToken);

        var dtos = allCategories.Select(c => new CategoryTreeDto
        {
            Id = c.Id,
            Name = c.Name,
            IconUrl = c.IconUrl,
            ParentId = c.ParentId,
            Depth = c.Depth
        }).ToList();

        var lookup = dtos.ToDictionary(c => c.Id);
        CategoryTreeDto? root = null;

        foreach (CategoryTreeDto cat in lookup.Values)
        {
            if (cat.ParentId is not null && lookup.TryGetValue(cat.ParentId.Value, out CategoryTreeDto? parent))
            {
                parent.Children ??= new List<CategoryTreeDto>();
                parent.Children.Add(cat);
            }

            if (cat.Id == category.Id)
            {
                root = cat;
            }
        }

        return Result.Success<object>(root is not null ? root : category.ToCategoryDto());

    }
}
