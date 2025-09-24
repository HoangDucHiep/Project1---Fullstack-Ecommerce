using ECommerceBackend.Common.Domain;
using ECommerceBackend.Common.Domain.Utils;

namespace ECommerceBackend.Modules.Users.Domain.Users;


/// HoangDucHiep - 08/2025
public sealed class User : Entity
{
    private User()
    {
    }

    public Guid Id { get; private set; }
    public string IdentityId { get; private set; }

    public string Email { get; private set; }
    public string Phone { get; private set; }


    public static User Create(string phone, string identityId)
    {
        var user = new User
        {
            Id = IdGenerator.GenerateId(),
            Phone = phone,
            IdentityId = identityId
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void Update(string email)
    {
        Email = email;

        Raise(new UserProfileUpdatedDomainEvent(Id, Email));
    }
}

