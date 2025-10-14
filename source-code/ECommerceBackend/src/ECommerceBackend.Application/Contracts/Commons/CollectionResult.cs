namespace ECommerceBackend.Application.Contracts.Commons;


/// HDHiep - 10/06/2025
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class CollectionResult<T> : ICollectionResponse<T>
{
    public List<T> Items { get; init; }

    public static CollectionResult<T> CreateAsync(List<T> items)
    {
        return new CollectionResult<T>
        {
            Items = items
        };
    }
}
