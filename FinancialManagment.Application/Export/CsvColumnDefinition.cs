using System.Reflection;

namespace FinancialManagment.Application.Export;

public sealed class CsvColumnDefinition
{
    public string Header { get; set; } = string.Empty;
    public PropertyInfo PropertyInfo { get; set; } = default!;
    public int Order { get; set; }
}