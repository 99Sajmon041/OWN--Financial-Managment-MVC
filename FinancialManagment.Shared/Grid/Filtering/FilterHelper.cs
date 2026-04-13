namespace FinancialManagment.Shared.Grid.Filtering;

public static class FilterHelper
{
    public static string? GetSelectedValue(List<FilterItem> filters, string propertyName)
    {
        return filters.FirstOrDefault(x => x.PropertyName == propertyName)?.Value;
    }

    public static FilterOperator GetSelectedOperator(List<FilterItem> filters, string propertyName)
    {
        return filters.FirstOrDefault(x => x.PropertyName == propertyName)?.Operator ?? FilterOperator.None;
    }
}
