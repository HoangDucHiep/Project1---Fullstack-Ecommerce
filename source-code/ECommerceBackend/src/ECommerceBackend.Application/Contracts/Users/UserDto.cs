namespace ECommerceBackend.Application.Contracts.Users;

/// <summary>
/// Data transfer object for user information
/// </summary>
public sealed record UserDto
{
    public Guid Id { get; init; }
    public string IdentityId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; }
    public IEnumerable<string> Roles { get; init; } = new List<string>();
}
