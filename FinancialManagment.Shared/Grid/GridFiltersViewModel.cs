namespace FinancialManagment.Shared.Grid;

public sealed class GridFiltersViewModel
{
    public List<FilterFieldDefinition> Fields { get; set; } = [];
    public string Action { get; set; } = default!;
    public string Controller { get; set; } = default!;
    public int PageSize { get; set; }
    public string? SortOrder { get; set; }
}
