using System.Globalization;
using System.Text;
using FinancialManagment.Application.Export;
using FinancialManagment.Application.Services.Interfaces;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class CsvExportService : ICsvExportService
{
    public byte[] ExportToCsv<T>(IEnumerable<T> items, IReadOnlyList<CsvColumnDefinition<T>> columns)
    {
        StringBuilder stringBuilder = new();

        var orderedColumns = columns
            .OrderBy(x => x.Order)
            .ToList();

        string headerLine = string.Join(";", orderedColumns.Select(x => EscapeValue(x.Header)));
        stringBuilder.AppendLine(headerLine);

        foreach (T item in items)
        {
            List<string> values = [];

            foreach (CsvColumnDefinition<T> column in orderedColumns)
            {
                object? rawValue = column.ValueSelector(item);
                string formattedValue = FormatValue(rawValue);

                values.Add(EscapeValue(formattedValue));
            }

            string line = string.Join(";", values);
            stringBuilder.AppendLine(line);
        }

        return Encoding.UTF8.GetBytes(stringBuilder.ToString());
    }

    private static string FormatValue(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (value is DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        if (value is DateOnly dateOnly)
        {
            return dateOnly.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        if (value is double doubleValue)
        {
            return doubleValue.ToString(CultureInfo.InvariantCulture);
        }

        if (value is decimal decimalValue)
        {
            return decimalValue.ToString(CultureInfo.InvariantCulture);
        }

        if (value is float floatValue)
        {
            return floatValue.ToString(CultureInfo.InvariantCulture);
        }

        return value.ToString() ?? string.Empty;
    }

    private static string EscapeValue(string value)
    {
        bool mustBeQuoted =
            value.Contains(';') ||
            value.Contains('"') ||
            value.Contains('\n') ||
            value.Contains('\r');

        if (!mustBeQuoted)
        {
            return value;
        }

        string escapedValue = value.Replace("\"", "\"\"");

        return $"\"{escapedValue}\"";
    }
}
