namespace ECommerceBackend.Domain.Utils;


/// HDHIep - 09/24/2025
public static class IdGenerator
{
    /// <summary>
    /// Generates a new unique identifier (ID) using the version 7 format.
    /// </summary>
    /// <returns>A new GUID representing the unique identifier.</returns>
    public static Guid GenerateId() => Guid.CreateVersion7();
}
