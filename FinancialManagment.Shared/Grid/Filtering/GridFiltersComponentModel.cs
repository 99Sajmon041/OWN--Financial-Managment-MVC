namespace FinancialManagment.Shared.Grid.Filtering;

public sealed class GridFiltersComponentModel
{
    public Type ModelType { get; set; } = default!;
    public List<FilterItem> Filters { get; set; } = [];
    public string Action { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public int PageSize { get; set; }
    public string? SortOrder { get; set; }
    public bool FiltersCollapsed { get; set; }
    public List<FilterFieldDefinition> CustomFilters { get; set; } = [];
}
