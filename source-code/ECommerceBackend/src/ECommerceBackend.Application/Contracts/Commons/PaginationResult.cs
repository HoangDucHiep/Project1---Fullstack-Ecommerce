using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Application.Contracts.Commons;

/// HDHiep - 10/01/2025
/// <summary>
/// Represents a paginated result containing a collection of items with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the paginated collection.</typeparam>
public sealed record PaginationResult<T> : ICollectionResponse<T> //, ILinksResponse
{
    /// <summary>
    /// Gets the collection of items for the current page.
    /// </summary>
    public List<T> Items { get; init; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the total number of pages based on the total count and page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page available.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page available.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Creates a paginated result asynchronously from an IQueryable source.
    /// </summary>
    /// <param name="query">The queryable source to paginate.</param>
    /// <param name="page">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task that represents the asynchronous operation, containing the paginated result.</returns>
    public static async Task<PaginationResult<T>> CreateAsync(
        IQueryable<T> query,
        int page,
        int pageSize)
    {
        int totalCount = await query.CountAsync();
        List<T> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Creates a paginated result from an existing collection of items.
    /// </summary>
    /// <param name="items">The collection of items for the current page - which is already paginated.</param>
    /// <param name="page">The current page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <returns>A paginated result containing the specified items and metadata.</returns>
    public static PaginationResult<T> CreateAsync(
        IEnumerable<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        return new PaginationResult<T>
        {
            Items = [.. items],
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
