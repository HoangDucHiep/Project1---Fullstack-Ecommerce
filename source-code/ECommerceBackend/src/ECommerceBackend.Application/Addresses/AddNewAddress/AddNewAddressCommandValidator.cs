using FluentValidation;

namespace ECommerceBackend.Application.Addresses.AddNewAddress;
internal sealed class AddNewAddressCommandValidator : AbstractValidator<AddNewAddressCommand>
{
    public AddNewAddressCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Bắt buộc phải nhập họ tên.")
            .MinimumLength(5).WithMessage("Họ tên phải dài ít nhất 5 kí tự.")
            .MaximumLength(100).WithMessage("Họ tên không được dài hơn 100 kí tự.");
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Bắt buộc phải nhập số điện thoại.")
            .Matches(@"^(84|0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-9]|9[0-9])[0-9]{7}$").WithMessage("Số điện thoại không hợp lệ.");
        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Bắt buộc phải nhập tỉnh/thành phố.")
            .MaximumLength(100).WithMessage("Tỉnh/thành phố không được dài hơn 100 kí tự.");
        RuleFor(x => x.District)
            .NotEmpty().WithMessage("Bắt buộc phải nhập quận/huyện.")
            .MaximumLength(100).WithMessage("Quận/huyện không được dài hơn 100 kí tự.");
        RuleFor(x => x.Ward)
            .NotEmpty().WithMessage("Bắt buộc phải nhập phường/xã.")
            .MaximumLength(100).WithMessage("Phường/xã không được dài hơn 100 kí tự.");
        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Bắt buộc phải nhập địa chỉ cụ thể.")
            .MaximumLength(200).WithMessage("Địa chỉ cụ thể không được dài hơn 200 kí tự.");
        RuleFor(x => x.IsDefault)
            .NotNull().WithMessage("Bắt buộc phải chỉ định địa chỉ mặc định.");
        RuleFor(x => x.IsPickUpAddress)
            .NotNull().WithMessage("Bắt buộc phải chỉ định địa chỉ lấy hàng.");
        RuleFor(x => x.IsReturnAddress)
            .NotNull().WithMessage("Bắt buộc phải chỉ định địa chỉ trả hàng.");
    }
}
