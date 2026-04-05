namespace FinancialManagment.Shared.Grid;

public sealed class FilterFieldDefinition
{
    public string PropertyName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public Type PropertyType { get; set; } = default!;
    public FilterInputType InputType { get; set; }
    public string? Value { get; set; }
}
