namespace FinancialManagment.Shared.Grid.Paging;

public sealed class PagerPartialViewModel
{
    public Pager Pager { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string Controller { get; set; } = default!;
    public string? SortOrder { get; set; }
    public int PageSize { get; set; }
    public bool FiltersCollapsed { get; set; }
    public Dictionary<string, string> Filters { get; set; } = [];
}