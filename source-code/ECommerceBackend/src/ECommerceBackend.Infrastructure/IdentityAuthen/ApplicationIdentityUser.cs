using Microsoft.AspNetCore.Identity;

namespace ECommerceBackend.Infrastructure.Identity;

/// HDHiep - 10/06/2025
/// <summary>
/// Custom Application User class extending IdentityUser to include additional properties
/// </summary>
public class ApplicationIdentityUser : IdentityUser
{
    public override string? Email { get; set; }
    public override bool EmailConfirmed { get; set; }
    public override string? PhoneNumber { get; set; }
    public override bool PhoneNumberConfirmed { get; set; }

}
