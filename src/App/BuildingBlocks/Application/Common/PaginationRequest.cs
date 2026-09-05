namespace App.BuildingBlocks.Application.Common;

public sealed class PaginationRequest
{
    private const int MaxPageSize = 100;
    private readonly int _pageSize = 20;

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value is > 0 and <= MaxPageSize ? value : MaxPageSize;
    }

    public int Skip => (Math.Max(Page, 1) - 1) * PageSize;
}
