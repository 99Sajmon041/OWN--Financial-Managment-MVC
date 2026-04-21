using FinancialManagment.Application.Export;

namespace FinancialManagment.Application.Services.Interfaces;

public interface ICsvExportService
{
    byte[] ExportToCsv<T>(IEnumerable<T> items, IReadOnlyList<CsvColumnDefinition> columns);
}
