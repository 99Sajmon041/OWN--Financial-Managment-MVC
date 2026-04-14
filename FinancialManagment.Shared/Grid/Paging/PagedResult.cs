using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Filtering;

namespace FinancialManagment.Shared.Grid.Paging;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public Pager Pager { get; set; } = default!;
    public GridRequest GridRequest { get; set; } = new();
    public Type? FilterModelType { get; set; }
    public List<FilterFieldDefinition> CustomFilters { get; set; } = [];
}
