using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;

namespace ECommerceBackend.Application.Addresses.AddNewAddress;

/// HDHiep - 10/01/2025
/// <summary>
/// Command to add a new address for current user
/// </summary>
/// <param name="Name"> The name of the recipient </param>
/// <param name="Phone"> The phone number of the recipient </param>
/// <param name="Province"> The province of the address </param>
/// <param name="District"> The district of the address </param>
/// <param name="Ward"> The ward of the address </param>
/// <param name="AddressLine"> The detailed address line </param>
/// <param name="IsDefault"> Indicates if this is the default address </param>
/// <param name="IsPickUpAddress"> Indicates if this is a pick-up address </param>
/// <param name="IsReturnAddress"> Indicates if this is a return address </param>
public sealed record AddNewAddressCommand(
    string Name,
    string Phone,
    string Province,
    string District,
    string Ward,
    string AddressLine,
    bool IsDefault,
    bool IsPickUpAddress,
    bool IsReturnAddress
    ) : ICommand<AddressDto>;

