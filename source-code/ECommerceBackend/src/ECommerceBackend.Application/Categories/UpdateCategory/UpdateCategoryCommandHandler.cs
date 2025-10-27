using System;
using System.Threading;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.UpdateCategory;

public sealed class UpdateCategoryCommandHandler
    : ICommandHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.NotFound(command.Id));
        }

        bool duplicate = await _categoryRepository.ExistsByNameAsync(
            command.Name,
            category.ParentId,
            category.Id,
            cancellationToken);

        if (duplicate)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.DuplicateName(command.Name));
        }

        if (command.NewParentId != category.ParentId)
        {
            if (command.NewParentId is null)
            {
                category.MoveTo(null, 0);
                await _categoryRepository.UpdateChildrenDepthAsync(category.Id, category.Depth, cancellationToken);
            }
            else
            {
                if (command.NewParentId == category.Id)
                {
                    return Result.Failure<CategoryDto>(CategoryErrors.InvalidParent());
                }

                Category? newParent = await _categoryRepository.GetByIdAsync(
                    command.NewParentId.Value,
                    cancellationToken);

                if (newParent is null)
                {
                    return Result.Failure<CategoryDto>(CategoryErrors.InvalidParent());
                }

                bool isDescendant = await _categoryRepository.IsDescendantAsync(
                    category.Id,
                    command.NewParentId.Value,
                    cancellationToken);

                if (isDescendant)
                {
                    return Result.Failure<CategoryDto>(CategoryErrors.InvalidParent());
                }

                category.MoveTo(newParent.Id, newParent.Depth + 1);

                await _categoryRepository.UpdateChildrenDepthAsync(category.Id, category.Depth, cancellationToken);
            }
        }

        category.Rename(command.Name);

        if (command.IconUrl is not null)
        {
            category.Update(command.Name, command.IconUrl);
        }

        category.ChangeStatus(command.Status);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CategoryDto.From(category));
    }
}
