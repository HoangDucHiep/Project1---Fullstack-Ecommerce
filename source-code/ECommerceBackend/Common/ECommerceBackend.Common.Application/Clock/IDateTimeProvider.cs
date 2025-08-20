namespace ECommerceBackend.Common.Application.Clock;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
