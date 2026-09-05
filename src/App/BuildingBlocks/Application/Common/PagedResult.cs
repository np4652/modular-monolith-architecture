namespace App.BuildingBlocks.Application.Common;

/// <summary>
/// The pagination metadata of a <see cref="PagedResult{T}"/> without its item type,
/// so shared UI (e.g. a pagination partial view) can render it generically.
/// </summary>
public interface IPagedResult
{
    int TotalCount { get; }

    int Page { get; }

    int PageSize { get; }

    int TotalPages { get; }

    bool HasPreviousPage { get; }

    bool HasNextPage { get; }
}

public sealed class PagedResult<T> : IPagedResult
{
    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public IReadOnlyList<T> Items { get; }

    public int TotalCount { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}
