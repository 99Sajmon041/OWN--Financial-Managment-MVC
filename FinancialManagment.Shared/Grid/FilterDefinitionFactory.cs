using FinancialManagment.Shared.Attributes;
using System.Reflection;

namespace FinancialManagment.Shared.Grid;

public static class FilterDefinitionFactory
{
    public static List<FilterFieldDefinition> Create(Type modelType, Dictionary<string, string> filters)
    {
        List<FilterFieldDefinition> definitions = [];

        PropertyInfo[] properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            if (Attribute.IsDefined(property, typeof(NotFilterableAttribute)))
            {
                continue;
            }

            if (!IsSupportedFilterType(property.PropertyType))
            {
                continue;    
            }

            string label = property.GetCustomAttribute<FilterLabelAttribute>()?.Label ?? property.Name;
            string? value = null;

            if (filters.TryGetValue(property.Name, out string? filterValue))
            {
                value = filterValue;
            }

            var definition = new FilterFieldDefinition
            {
                PropertyName = property.Name,
                Label = label,
                PropertyType = property.PropertyType,
                InputType = ResolveInputType(property.PropertyType),
                Value = value
            };

            definitions.Add(definition);
        }

        return definitions;
    }

    private static bool IsSupportedFilterType(Type propertyType)
    {
        Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        return underlyingType == typeof(string)
            || underlyingType == typeof(bool)
            || underlyingType == typeof(int)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(DateTime);
    }

    private static FilterInputType ResolveInputType(Type propertyType)
    {
        Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlyingType == typeof(bool))
        {
            return FilterInputType.Select;
        }

        if (underlyingType == typeof(int) || underlyingType == typeof(decimal))
        {
            return FilterInputType.Number;
        }

        if (underlyingType == typeof(DateTime))
        {
            return FilterInputType.Date;
        }

        return FilterInputType.Text;
    }
}
