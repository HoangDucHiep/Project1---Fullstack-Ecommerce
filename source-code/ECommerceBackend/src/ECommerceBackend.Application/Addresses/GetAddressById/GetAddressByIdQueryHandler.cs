using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.GetAddressById;
internal sealed class GetAddressByIdQueryHandler : IQueryHandler<GetAddressByIdQuery, AddressDto>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetAddressByIdQueryHandler(IDbConnectionFactory dbConnection)
    {
        _dbConnectionFactory = dbConnection;
    }

    public async Task<Result<AddressDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Kiểm tra có là của User hiện tại hay không


        DbConnection dbConnection = await _dbConnectionFactory.OpenConnectionAsync();

        string sql = $"""
            SELECT 
                id AS {nameof(AddressDto.Id)},
                user_id AS {nameof(AddressDto.UserId)},
                name AS {nameof(AddressDto.Name)},
                phone AS {nameof(AddressDto.Phone)},
                province AS {nameof(AddressDto.Province)},
                district AS {nameof(AddressDto.District)},
                ward AS {nameof(AddressDto.Ward)},
                address_line AS {nameof(AddressDto.AddressLine)},
                is_default AS {nameof(AddressDto.IsDefault)},
                is_pick_up_address AS {nameof(AddressDto.IsPickUpAddress)},
                is_return_address AS {nameof(AddressDto.IsReturnAddress)}
            FROM addresses
            WHERE id = @AddressId
        """;

        AddressDto? address = await dbConnection.QuerySingleOrDefaultAsync<AddressDto>(sql, new { request.AddressId });

        if (address == null)
        {
            return Result.Failure<AddressDto>(AddressErrors.NotFound());
        }

        return Result.Success(address);
    }
}
