namespace ECommerceBackend.Domain.Addresses;
public interface IAddressRepository
{
    Task<List<Address>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Address address);

    Task<List<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    int Update(Address address);

    void Delete(Address address);
}
