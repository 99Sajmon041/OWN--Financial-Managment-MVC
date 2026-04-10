namespace FinancialManagment.Shared.Grid.Filtering;

public sealed class FilterFieldDefinition
{
    public string PropertyName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string PropertyPath { get; set; } = string.Empty;
    public Type PropertyType { get; set; } = default!;
    public Type UnderlyingType { get; set; } = default!;
    public FilterInputType InputType { get; set; }
    public List<FilterOperator> AllowedOperators { get; set; } = [];
    public FilterOperator SelectedOperator { get; set; }
    public string? Value { get; set; }
    public int Order { get; set; } = int.MaxValue;
    public string GroupName { get; set; } = string.Empty;
}
