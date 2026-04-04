namespace FinancialManagment.Shared.Grid;

public static class QueryablePagingExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, Pager pager)
    {
        return query
            .Skip(pager.GetSkip())
            .Take(pager.PageSize);
    }
}
