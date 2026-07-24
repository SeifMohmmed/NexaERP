using Microsoft.EntityFrameworkCore;

namespace NexaERP.BLL.DTOs.Common;

/// <summary>
/// Represents a paginated response.
/// </summary>
public sealed record PaginationResult<T> : ICollectionResponse<T>, ILinksResponse
{
    // Current page items.
    public List<T> Items { get; init; }

    // Current page number.
    public int Page { get; init; }

    // Number of items per page.
    public int PageSize { get; init; }

    // Total number of items.
    public int TotalCount { get; init; }

    // Total number of pages.
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    // Indicates whether a previous page exists.
    public bool HasPreviousPage => Page > 1;

    // Indicates whether a next page exists.
    public bool HasNextPage => Page < TotalPages;

    // Collection links.
    public List<LinkDto> Links { get; set; }

    // Creates a paginated result from a query.
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
}
