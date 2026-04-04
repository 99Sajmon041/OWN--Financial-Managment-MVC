using System.Linq.Expressions;

namespace FinancialManagment.Shared.Grid;

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
        string propertyName = parts[1];

        if (direction != "asc" && direction != "desc")
        {
            return query;
        }

        var property = typeof(T).GetProperty(propertyName);

        if (property == null)
        {
            return query;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression propertyExpression = Expression.Property(parameter, property);
        LambdaExpression lambda = Expression.Lambda(propertyExpression, parameter);

        string methodName = direction == "asc" ? "OrderBy" : "OrderByDescending";

        MethodCallExpression methodCallExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(methodCallExpression);
    }
}
