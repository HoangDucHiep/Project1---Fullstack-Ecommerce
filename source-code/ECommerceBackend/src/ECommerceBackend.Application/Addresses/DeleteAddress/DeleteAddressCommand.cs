using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Addresses.DeleteAddress;
public sealed record DeleteAddressCommand(Guid AddressId, Guid UserId) : ICommand;
