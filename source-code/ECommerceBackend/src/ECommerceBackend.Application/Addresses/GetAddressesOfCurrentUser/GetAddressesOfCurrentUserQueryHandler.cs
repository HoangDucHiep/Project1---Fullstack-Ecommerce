using System.Data.Common;
using Dapper;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.GetAddressesOfCurrentUser;
internal sealed class GetAddressesOfCurrentUserQueryHandler : IQueryHandler<GetAddressesOfCurrentUserQuery, PaginationResult<AddressDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IUserContext _userContext;

    public GetAddressesOfCurrentUserQueryHandler(IDbConnectionFactory dbConnectionFactory, IUserContext userContext)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _userContext = userContext;
    }

    public async Task<Result<PaginationResult<AddressDto>>> Handle(GetAddressesOfCurrentUserQuery request, CancellationToken cancellationToken)
    {
        DbConnection dbConnection = await _dbConnectionFactory.OpenConnectionAsync();

        if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
        {
            return Result.Failure<PaginationResult<AddressDto>>(AddressErrors.Unauthorized());
        }


        var userId = Guid.Parse(_userContext.UserId!);

        string mainSql = $"""
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
            FROM "ecommerce-domain".addresses
            WHERE user_id = @userId
            LIMIT @PageSize OFFSET (@Page - 1) * @PageSize
            """;

        string countSql = $"""
            SELECT COUNT(*) 
            FROM "ecommerce-domain".addresses
            WHERE user_id = @userId
            """;

        // Do pagination later
        IEnumerable<AddressDto> addresses = await dbConnection.QueryAsync<AddressDto>(mainSql, new { userId, request.Page, request.PageSize });
        int totalCount = await dbConnection.ExecuteScalarAsync<int>(countSql, new { userId });

        var paginationResult = PaginationResult<AddressDto>.CreateAsync(addresses, request.Page, request.PageSize, totalCount);

        return Result.Success(paginationResult);
    }
}
