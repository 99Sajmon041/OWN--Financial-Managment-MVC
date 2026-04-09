using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

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

            Expression<Func<T, bool>>? expression = BuildExpression<T>(property, filter.Value, filter.Operator);

            if (expression != null)
            {
                expressions.Add(expression);
            }
        }

        return expressions;
    }

    private static Expression<Func<T, bool>>? BuildExpression<T>(PropertyInfo property, string value, FilterOperator filterOperator)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression propertyExpression = Expression.Property(parameter, property);
        Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (underlyingType == typeof(string))
        {
            ConstantExpression constant = Expression.Constant(value, typeof(string));
            Expression body;

            if (filterOperator == FilterOperator.Equal)
            {
                body = Expression.Equal(propertyExpression, constant);
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

            if (property.PropertyType == typeof(bool?))
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

            if (property.PropertyType == typeof(int?))
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

            if (property.PropertyType == typeof(decimal?))
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

            if (property.PropertyType == typeof(DateTime?))
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
            BinaryExpression comparsionExpression;

            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                constant = Expression.Constant(parsedValue, property.PropertyType);
            }
            else
            {
                constant = Expression.Constant(parsedValue, underlyingType);
            }

            if (filterOperator == FilterOperator.NotEqual)
            {
                comparsionExpression = Expression.NotEqual(propertyExpression, constant);
            }
            else
            {
                comparsionExpression = Expression.Equal(propertyExpression, constant);
            }

            return Expression.Lambda<Func<T, bool>>(comparsionExpression, parameter);
        }

        return null;
    }
}
