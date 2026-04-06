using System.Linq.Expressions;

namespace FinancialManagment.Shared.Grid.Filtering;

public static class QueryableFilterExtensions
{
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string> filters)
    {
        if (filters == null || filters.Count == 0)
        {
            return query;
        }

        List<Expression<Func<T, bool>>> expressions = FilterExpressionBuilder.Build<T>(filters);

        foreach (Expression<Func<T, bool>> expression in expressions)
        {
            query = query.Where(expression);
        }

        return query;
    }
}
