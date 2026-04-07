using FinancialManagment.Shared.Grid.Filtering;

namespace FinancialManagment.Shared.Grid.Common;

public sealed class GridRequest
{
    public List<FilterItem> Filters { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortOrder { get; set; }
    public bool FiltersCollapsed { get; set; }
    public void Normalize()
    {
        if (Page < 1)
        {
            Page = 1;
        }

        if (PageSize < 1)
        {
            PageSize = 10;
        }
    }
}
