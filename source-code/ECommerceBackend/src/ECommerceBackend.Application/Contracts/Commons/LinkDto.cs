namespace ECommerceBackend.Application.Contracts.Commons;
public sealed record LinkDto(
    string Href,
    string Rel,
    string Method
);
