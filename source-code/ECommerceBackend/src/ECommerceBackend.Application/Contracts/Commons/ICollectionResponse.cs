namespace ECommerceBackend.Application.Contracts.Commons;

/// HDHiep - 10/01/2025
/// <summary>
/// Represents a response containing a collection of items.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}

