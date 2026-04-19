namespace FinancialManagment.Application.Export;

public sealed class CsvColumnDefinition<T>
{
    public string Header { get; set; } = string.Empty;
    public Func<T, object?> ValueSelector { get; set; } = _ => null;
    public int Order { get; set; }
}
