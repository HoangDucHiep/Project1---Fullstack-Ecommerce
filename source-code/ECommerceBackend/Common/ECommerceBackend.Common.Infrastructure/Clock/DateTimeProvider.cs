using ECommerceBackend.Common.Application.Clock;

namespace ECommerceBackend.Common.Infrastructure.Clock;

/// <summary>
/// Provides the current UTC time using <see cref="DateTimeOffset.UtcNow"/>.
/// </summary>
internal sealed class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
