using FinancialManagment.Shared.Attributes;
using System.Reflection;

namespace FinancialManagment.Shared.Grid.Filtering;

public static class FilterDefinitionFactory
{
    public static List<FilterFieldDefinition> Create(Type modelType, List<FilterItem> filters)
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

            Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            FilterOperator filterOperator = ResolveDefaultOperator(underlyingType);

            foreach (var filter in filters)
            {
                if (filter.PropertyName == property.Name)
                { 
                    value = filter.Value;
                    filterOperator = filter.Operator;
                    break;
                }
            }

            int order = property.GetCustomAttribute<FilterOrderAttribute>()?.Order ?? int.MaxValue;

            FilterInputType inputType = property.GetCustomAttribute<FilterTypeAttribute>()?.InputType ?? ResolveInputType(property.PropertyType);

            var definition = new FilterFieldDefinition
            {
                PropertyName = property.Name,
                PropertyPath = property.Name,
                Label = label,
                PropertyType = property.PropertyType,
                UnderlyingType = underlyingType,
                InputType = inputType,
                AllowedOperators = ResolveAllowedOperators(underlyingType),
                SelectedOperator = filterOperator,
                Value = value,
                Order = order
            };

            definitions.Add(definition);
        }

        return definitions
            .OrderBy(x => x.Order)
            .ThenBy(x => x.PropertyName)
            .ToList();
    }

    private static FilterOperator ResolveDefaultOperator(Type underlyingType)
    {
        if (underlyingType == typeof(string))
        {
            return FilterOperator.Contains;
        }
        else
        {
            return FilterOperator.Equal;
        }
    }

    private static List<FilterOperator> ResolveAllowedOperators(Type underlyingType)
    {
        List<FilterOperator> operators =
        [
            FilterOperator.None
        ];

        if (underlyingType == typeof(string))
        {
            operators.Add(FilterOperator.Contains);
            operators.Add(FilterOperator.Equal);
            operators.Add(FilterOperator.StartsWith);
            operators.Add(FilterOperator.EndsWith);
        }
        else if (underlyingType == typeof(bool))
        {
            operators.Add(FilterOperator.Equal);
            operators.Add(FilterOperator.NotEqual);
        }
        else if (underlyingType == typeof(int) || underlyingType == typeof(decimal) || underlyingType == typeof(DateTime))
        {
            operators.Add(FilterOperator.Equal);
            operators.Add(FilterOperator.NotEqual);
            operators.Add(FilterOperator.LessThanOrEqual);
            operators.Add(FilterOperator.LessThan);
            operators.Add(FilterOperator.GreaterThan);
            operators.Add(FilterOperator.GreaterThanOrEqual);
        }
        else if (underlyingType.IsEnum)
        {
                operators.Add(FilterOperator.Equal);
                operators.Add(FilterOperator.NotEqual);
        }

        return operators;
    }

    private static bool IsSupportedFilterType(Type propertyType)
    {
        Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        return underlyingType == typeof(string)
            || underlyingType == typeof(bool)
            || underlyingType == typeof(int)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(DateTime)
            || underlyingType.IsEnum;
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

        if (underlyingType.IsEnum)
        {
            return FilterInputType.Select;
        }

        return FilterInputType.Text;
    }
}
