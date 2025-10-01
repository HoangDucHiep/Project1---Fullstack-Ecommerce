using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Addresses;
public static class AddressErrors
{
    public static Error NotFound() =>
        Error.NotFound("Addresses.NotFound", $"Không tìm thấy địa chỉ với thông tin được cung cấp");
}
