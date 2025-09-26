namespace ECommerceBackend.Application.Abstracts.Clock;

/// HDHiep - 09/24/2025
/// <summary>
/// Abstraction for obtaining current UTC time.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
