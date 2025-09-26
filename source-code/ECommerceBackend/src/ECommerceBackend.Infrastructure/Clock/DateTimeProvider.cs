using ECommerceBackend.Application.Abstracts.Clock;

namespace ECommerceBackend.Infrastructure.Clock;

/// HDHiep - 09/24/2025
/// <summary>
/// Provides the current UTC time using <see cref="DateTimeOffset.UtcNow"/>.
/// </summary>
internal sealed class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC time with offset.
    /// </summary>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
