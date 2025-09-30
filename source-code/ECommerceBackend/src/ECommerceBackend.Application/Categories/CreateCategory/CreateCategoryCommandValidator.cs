using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Application.Categories.CategoryRegister;
using FluentValidation;


namespace ECommerceBackend.Application.Categories.CategoryRegister;
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục là bắt buộc.")
            .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự.");

        RuleFor(x => x.IconUrl)
            .NotEmpty().WithMessage("Đường dẫn Icon là bắt buộc.")
            .MaximumLength(250).WithMessage("Đường dẫn Icon không được vượt quá 250 ký tự.");

        RuleFor(x => x.ParentId)
            .NotEmpty().WithMessage("Danh mục cha là bắt buộc.");

        RuleFor(x => x.Lft)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị Lft phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.Rgt)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị Rgt phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.Depth)
            .GreaterThanOrEqualTo(0).WithMessage("Độ sâu (Depth) phải lớn hơn hoặc bằng 0.");
    }
}

