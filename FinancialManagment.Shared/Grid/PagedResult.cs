namespace FinancialManagment.Shared.Grid;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public Pager Pager { get; set; } = default!;
    public GridRequest GridRequest { get; set; } = new();
    public Type? FilterModelType { get; set; }
}
