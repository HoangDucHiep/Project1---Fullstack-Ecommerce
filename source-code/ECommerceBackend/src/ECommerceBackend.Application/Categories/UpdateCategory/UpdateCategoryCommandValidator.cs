using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ECommerceBackend.Application.Categories.UpdateCategory;
public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(c => c.Status)
            .IsInEnum().WithMessage("Invalid category status.");

        RuleFor(c => c.IconUrl)
            .MaximumLength(255).WithMessage("Icon URL must not exceed 255 characters.")
            .When(c => !string.IsNullOrWhiteSpace(c.IconUrl));

        RuleFor(c => c)
            .Must(c => c.NewParentId != c.Id)
            .WithMessage("A category cannot be its own parent.");
        RuleFor(c => c.NewParentId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("Invalid parent category ID.");
    }
}
