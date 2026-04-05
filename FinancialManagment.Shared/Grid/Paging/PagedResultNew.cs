using FinancialManagment.Shared.Grid.Common;

namespace FinancialManagment.Shared.Grid.Paging;

public sealed class PagedResultNew<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public Pager Pager { get; set; } = default!;
    public GridRequest GridRequest { get; set; } = new();
    public Type? FilterModelType { get; set; }
}
