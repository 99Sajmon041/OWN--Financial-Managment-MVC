namespace FinancialManagment.Shared.Pagination;

public sealed class PagedResult<T> where T : class
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int TotalPages => PageSize == 0 ? 1 : (int)Math.Ceiling((double)TotalItemsCount / PageSize);
    public int TotalItemsCount { get; init; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
    public string? Search { get; init; }
    public string? SortBy { get; init; }
    public bool Desc { get; init; } = true;
    public List<T> Items { get; init; } = [];
}
