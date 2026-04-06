namespace FinancialManagment.Shared.Grid.Filtering;

public sealed class GridFiltersViewModel
{
    public List<FilterFieldDefinition> Fields { get; set; } = [];
    public string Action { get; set; } = default!;
    public string Controller { get; set; } = default!;
    public int PageSize { get; set; }
    public string? SortOrder { get; set; }
    public bool FiltersCollapsed { get; set; }
}
