namespace ECommerceBackend.Application.Contracts.Commons;

internal interface IPaginableQuery
{
    int Page { get; init; }
    int PageSize { get; init; }
}

