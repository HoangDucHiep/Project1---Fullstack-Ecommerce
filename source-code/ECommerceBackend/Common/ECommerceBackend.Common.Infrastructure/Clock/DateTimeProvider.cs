using ECommerceBackend.Common.Application.Clock;

namespace ECommerceBackend.Common.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
