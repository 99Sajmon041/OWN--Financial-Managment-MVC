using System.Globalization;
using System.Text;
using FinancialManagment.Application.Export;
using FinancialManagment.Application.Services.Interfaces;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class CsvExportService : ICsvExportService
{
    public byte[] ExportToCsv<T>(IEnumerable<T> items, IReadOnlyList<CsvColumnDefinition<T>> columns)
    {
        throw new NotImplementedException();
    }
}
