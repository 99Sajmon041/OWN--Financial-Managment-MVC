namespace FinancialManagment.Shared.Pagination;

public sealed class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool Desc { get; set; } = true;
}
