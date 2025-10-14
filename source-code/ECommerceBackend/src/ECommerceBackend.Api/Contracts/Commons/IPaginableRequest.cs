namespace ECommerceBackend.Api.Contracts.Commons;

public interface IPaginableRequest
{
    int Page { get; init; }
    int PageSize { get; init; }
}
