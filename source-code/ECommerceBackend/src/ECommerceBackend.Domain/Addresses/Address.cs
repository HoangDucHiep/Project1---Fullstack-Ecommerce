using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Addresses;
public sealed class Address : Entity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public string Province { get; private set; }
    public string District { get; private set; }
    public string Ward { get; private set; }
    public string AddressLine { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsPickUpAddress { get; private set; }
    public bool IsReturnAddress { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private Address()
    {
        // For ORM
    }

    public static Address Create(Guid userId, string name, string phone, string province, string district, string ward, string addressLine, bool isDefault, bool isPickUpAddress, bool isReturnAddress)
    {
        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Phone = phone,
            Province = province,
            District = district,
            Ward = ward,
            AddressLine = addressLine,
            IsDefault = isDefault,
            IsPickUpAddress = isPickUpAddress,
            IsReturnAddress = isReturnAddress,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        // Raise domain event if needed
        return address;
    }
}
