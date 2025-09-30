using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Api.Controllers.Categories.CategoryRegister;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Users.RegisterUser;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Categories.CategoryRegister;
internal sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private const int MaxCategoryNameLength = 100;
    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validation
        //Rỗng: EmptyName TC-POST-01
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure<Guid>(CategoryErrors.EmptyName());
        }

        //Tên dài: NameTooLong TC-POST-01
        if (request.Name.Length > MaxCategoryNameLength)
        {
            return Result.Failure<Guid>(CategoryErrors.NameTooLong(100));
        }

        // Trùng tên trong cha: DuplicateName / TC-POST-02
        List<Category> categories = await _categoryRepository.GetAllAsync(cancellationToken);
        bool exists = categories.Any(c =>
            c.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase) &&
            c.ParentId == request.ParentId);

        if (exists)
        { 
            return Result.Failure<Guid>(CategoryErrors.DuplicateName(request.Name, request.ParentId));
        }

        // TC-POST-03 – Parent không tồn tại  ParentNotFound

        if (request.ParentId.HasValue && !categories.Any(c => c.Id == request.ParentId.Value))
        {
            return Result.Failure<Guid>(CategoryErrors.ParentNotFound(request.ParentId.Value));
        }




        // TC-POST-04 – Buyer/Seller gọi → AccessDenied


        //  Icon URL hợp lệ (nếu có): InvalidIconUrl
        //if (!string.IsNullOrEmpty(request.IconUrl) &&
        //    !Uri.IsWellFormedUriString(request.IconUrl, UriKind.Absolute))
        //{
        //    return Result.Failure<Guid>(CategoryErrors.InvalidIconUrl(request.IconUrl));
        //}

        //// 7. Tree position hợp lệ
        //if (request.Lft >= request.Rgt)
        //{
        //    return Result.Failure<Guid>(CategoryErrors.InvalidTreePosition(request.Lft, request.Rgt));
        //}

        var category = Category.Create(
            request.Name,
            request.IconUrl,
            request.ParentId ?? Guid.Empty,
            request.Lft,
            request.Rgt,
            request.Depth
        );

        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(category.Id);
    }
}
