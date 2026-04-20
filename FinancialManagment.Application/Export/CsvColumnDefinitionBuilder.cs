using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FinancialManagment.Application.Export;

public static class CsvColumnDefinitionBuilder
{
    public static List<CsvColumnDefinition<T>> Build<T>()
    {
        List<CsvColumnDefinition<T>> columns = [];

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            string header = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name;

            columns.Add(new CsvColumnDefinition<T>
            {
                Header = header,
                Order = int.MaxValue,
                ValueSelector = item => property.GetValue(item)
            });
        }

        return columns;
    }
}