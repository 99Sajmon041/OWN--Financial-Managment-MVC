using System.Linq.Expressions;
using System.Reflection;

namespace FinancialManagment.Shared.Grid.Common;

public static class QueryableSortExtensions
{
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortOrder))
        {
            sortOrder = "asc_Id";
        }

        string[] parts = sortOrder.Split('_', 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            return query;
        }

        string direction = parts[0].ToLowerInvariant();
        string propertyPath = parts[1];

        if (direction != "asc" && direction != "desc")
        {
            return query;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression? propertyExpression = BuildPropertyExpression(parameter, propertyPath);

        if (propertyExpression == null)
        {
            return query;
        }

        LambdaExpression lambda = Expression.Lambda(propertyExpression, parameter);

        string methodName = direction == "asc" ? "OrderBy" : "OrderByDescending";

        MethodCallExpression resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), propertyExpression.Type],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }


    private static Expression? BuildPropertyExpression(Expression parametr, string propertyPath)
    {
        string[] pathParts = propertyPath.Split('.');
        Expression currentExpression = parametr;

        foreach (string part in pathParts)
        {
            var property = currentExpression.Type.GetProperty(part);

            if (property == null)
            {
                return null;
            }

            currentExpression = Expression.Property(currentExpression, property);
        }

        return currentExpression;
    }
}
