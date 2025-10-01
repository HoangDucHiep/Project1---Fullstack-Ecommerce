using FluentValidation;

namespace ECommerceBackend.Application.Addresses.DeleteAddress;
internal sealed class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("Bắt buộc phải có AddressId.")
            .NotEqual(Guid.Empty).WithMessage("AddressId phải là một GUID hợp lệ.");
    }
}
