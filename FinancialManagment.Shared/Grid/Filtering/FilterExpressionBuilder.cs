using System.Linq.Expressions;
using System.Reflection;

namespace FinancialManagment.Shared.Grid.Filtering;
public static class FilterExpressionBuilder
{
    public static List<Expression<Func<T, bool>>> Build<T>(List<FilterItem> filters)
    {
        List<Expression<Func<T, bool>>> expressions = [];

        foreach (var filter in filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Value))
            {
                continue;
            }

            PropertyInfo? property = typeof(T).GetProperty(filter.PropertyName);

            if (property == null)
            {
                continue;
            }

            Expression<Func<T, bool>>? expression = BuildExpression<T>(property, filter.Value);

            if (expression != null)
            {
                expressions.Add(expression);
            }
        }

        return expressions;
    }


    private static Expression<Func<T, bool>>? BuildExpression<T>(PropertyInfo property, string value)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression propertyExpression = Expression.Property(parameter, property);

        if (property.PropertyType == typeof(string))
        {
            MethodInfo? containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });

            if (containsMethod == null)
            {
                return null;
            }

            ConstantExpression constant = Expression.Constant(value, typeof(string));
            MethodCallExpression containsExpression = Expression.Call(propertyExpression, containsMethod, constant);

            return Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
        }

        if (property.PropertyType == typeof(bool))
        {
            if (!bool.TryParse(value, out bool parsedValue))
            {
                return null;
            }

            ConstantExpression constant = Expression.Constant(parsedValue, typeof(bool));
            BinaryExpression equalExpression = Expression.Equal(propertyExpression, constant);

            return Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
        }

        if (property.PropertyType == typeof(bool?))
        {
            if (!bool.TryParse(value, out bool parsedValue))
            {
                return null;
            }

            ConstantExpression constant = Expression.Constant((bool?)parsedValue, typeof(bool?));
            BinaryExpression equalExpression = Expression.Equal(propertyExpression, constant);

            return Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
        }

        if (property.PropertyType == typeof(int))
        {
            if (!int.TryParse(value, out int parsedValue))
            {
                return null;
            }

            ConstantExpression constant = Expression.Constant(parsedValue, typeof(int));
            BinaryExpression equalExpression = Expression.Equal(propertyExpression, constant);

            return Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
        }

        if (property.PropertyType == typeof(int?))
        {
            if (!int.TryParse(value, out int parsedValue))
            {
                return null;
            }

            ConstantExpression constant = Expression.Constant((int?)parsedValue, typeof(int?));
            BinaryExpression equalExpression = Expression.Equal(propertyExpression, constant);

            return Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
        }

        return null;
    }
}
