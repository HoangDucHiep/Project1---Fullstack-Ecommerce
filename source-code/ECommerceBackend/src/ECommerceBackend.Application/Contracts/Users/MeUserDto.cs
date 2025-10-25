namespace ECommerceBackend.Application.Contracts.Users;

/// <summary>
/// Data transfer object for user information
/// </summary>
public sealed record MeUserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Avatar_Url { get; init; }
    public string? Bio { get; init; }
    public string? IdCardFullName { get; init; }
    public string? IdCardNumber { get; init; }
    public string? IdCardFullAddress { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; }

}
