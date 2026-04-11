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

            if (filter.Operator == FilterOperator.None)
            {
                continue;
            }

            Expression<Func<T, bool>>? expression = BuildExpression<T>(filter.PropertyPath, filter.Value, filter.Operator);

            if (expression != null)
            {
                expressions.Add(expression);
            }
        }

        return expressions;
    }

    private static Expression<Func<T, bool>>? BuildExpression<T>(string propertyPath, string value, FilterOperator filterOperator)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        Expression? propertyExpression = BuildPropertyExpression(parameter, propertyPath);

        if (propertyExpression == null)
        {
            return null;
        }

        Type propertyType = propertyExpression.Type;
        Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlyingType == typeof(string))
        {
            ConstantExpression constant = Expression.Constant(value, typeof(string));
            Expression body;

            if (filterOperator == FilterOperator.Equal)
            {
                body = Expression.Equal(propertyExpression, constant);
            }
            else if (filterOperator == FilterOperator.NotEqual)
            {
                body = Expression.NotEqual(propertyExpression, constant);
            }
            else
            {
                MethodInfo? method = filterOperator switch
                {
                    FilterOperator.StartsWith => typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) }),
                    FilterOperator.EndsWith => typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) }),
                    _ => typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })
                };

                if (method == null)
                {
                    return null;
                }

                body = Expression.Call(propertyExpression, method, constant);
            }

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        if (underlyingType == typeof(bool))
        {
            if (!bool.TryParse(value, out bool parsedValue))
            {
                return null;
            }

            ConstantExpression constant;
            BinaryExpression comparisonExpression;

            if (propertyType == typeof(bool?))
            {
                constant = Expression.Constant((bool?)parsedValue, typeof(bool?));
            }
            else
            {
                constant = Expression.Constant(parsedValue, typeof(bool));
            }

            if (filterOperator == FilterOperator.Equal)
            {
                comparisonExpression = Expression.Equal(propertyExpression, constant);
            }
            else
            {
                comparisonExpression = Expression.NotEqual(propertyExpression, constant);
            }

            return Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
        }

        if (underlyingType == typeof(int))
        {
            if (!int.TryParse(value, out int parsedValue))
            {
                return null;
            }

            ConstantExpression constant;
            BinaryExpression comparisonExpression;

            if (propertyType == typeof(int?))
            {
                constant = Expression.Constant((int?)parsedValue, typeof(int?));
            }
            else
            {
                constant = Expression.Constant(parsedValue, typeof(int));
            }

            comparisonExpression = filterOperator switch
            {
                FilterOperator.NotEqual => Expression.NotEqual(propertyExpression, constant),
                FilterOperator.LessThan => Expression.LessThan(propertyExpression, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(propertyExpression, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(propertyExpression, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyExpression, constant),
                _ => Expression.Equal(propertyExpression, constant)
            };

            return Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
        }

        if (underlyingType == typeof(decimal))
        {
            if (!decimal.TryParse(value, out decimal parsedValue))
            {
                return null;
            }

            ConstantExpression constant;
            BinaryExpression comparisonExpression;

            if (propertyType == typeof(decimal?))
            {
                constant = Expression.Constant((decimal?)parsedValue, typeof(decimal?));
            }
            else
            {
                constant = Expression.Constant(parsedValue, typeof(decimal));
            }

            comparisonExpression = filterOperator switch
            {
                FilterOperator.NotEqual => Expression.NotEqual(propertyExpression, constant),
                FilterOperator.LessThan => Expression.LessThan(propertyExpression, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(propertyExpression, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(propertyExpression, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyExpression, constant),
                _ => Expression.Equal(propertyExpression, constant)
            };

            return Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
        }

        if (underlyingType == typeof(DateTime))
        {
            if (!DateTime.TryParse(value, out DateTime parsedValue))
            {
                return null;
            }

            ConstantExpression constant;
            BinaryExpression comparisonExpression;

            if (propertyType == typeof(DateTime?))
            {
                constant = Expression.Constant((DateTime?)parsedValue, typeof(DateTime?));
            }
            else
            {
                constant = Expression.Constant(parsedValue, typeof(DateTime));
            }

            comparisonExpression = filterOperator switch
            {
                FilterOperator.NotEqual => Expression.NotEqual(propertyExpression, constant),
                FilterOperator.LessThan => Expression.LessThan(propertyExpression, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(propertyExpression, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(propertyExpression, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyExpression, constant),
                _ => Expression.Equal(propertyExpression, constant)
            };

            return Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
        }

        if (underlyingType.IsEnum)
        {
            object? parsedValue;

            try
            {
                parsedValue = Enum.Parse(underlyingType, value);
            }
            catch
            {
                return null;
            }

            ConstantExpression constant;
            BinaryExpression comparisonExpression;

            if (Nullable.GetUnderlyingType(propertyType) != null)
            {
                object nullableEnumValue = Activator.CreateInstance(propertyType, parsedValue)!;
                constant = Expression.Constant(nullableEnumValue, propertyType);
            }
            else
            {
                constant = Expression.Constant(parsedValue, underlyingType);
            }

            if (filterOperator == FilterOperator.NotEqual)
            {
                comparisonExpression = Expression.NotEqual(propertyExpression, constant);
            }
            else
            {
                comparisonExpression = Expression.Equal(propertyExpression, constant);
            }

            return Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
        }

        return null;
    }

    private static Expression? BuildPropertyExpression(Expression parameter, string propertyPath)
    {
        string[] pathParts = propertyPath.Split('.');
        Expression currentExpression = parameter;

        foreach (string part in pathParts)
        {
            PropertyInfo? property = currentExpression.Type.GetProperty(part);

            if (property == null)
            {
                return null;
            }

            currentExpression = Expression.Property(currentExpression, property);
        }

        return currentExpression;
    }
}