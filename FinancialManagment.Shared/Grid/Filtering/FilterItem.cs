namespace FinancialManagment.Shared.Grid.Filtering;

public sealed class FilterItem
{
    public string PropertyName { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; }
    public string Value { get; set; } = string.Empty;
}
