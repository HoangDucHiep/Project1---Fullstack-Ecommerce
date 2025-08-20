namespace ECommerceBackend.Common.Application.Clock;

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
