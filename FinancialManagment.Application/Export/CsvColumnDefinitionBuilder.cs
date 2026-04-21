using System.Reflection;

namespace FinancialManagment.Application.Export;

public static class CsvColumnDefinitionBuilder
{
    public static List<CsvColumnDefinition> Build<T>()
    {
        List<CsvColumnDefinition> columns = [];

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            columns.Add(new CsvColumnDefinition
            {
                Header = property.Name,
                Order = int.MaxValue,
                PropertyInfo = property
            });
        }

        return columns;
    }
}