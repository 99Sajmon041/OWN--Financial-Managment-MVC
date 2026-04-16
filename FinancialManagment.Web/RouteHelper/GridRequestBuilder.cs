using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Filtering;

namespace FinancialManagment.Web.RouteHelper;

public static class GridRequestBuilder
{
    public static Dictionary<string, string> GetRouteValues(GridRequest gridRequest, string sortValue)
    {
        Dictionary<string, string> routeValues = new()
        {
            ["page"] = "1",
            ["pageSize"] = gridRequest.PageSize.ToString(),
            ["filtersCollapsed"] = gridRequest.FiltersCollapsed.ToString().ToLower(),
            ["sortOrder"] = sortValue
        };

        foreach (var filter in gridRequest.Filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Value))
            {
                continue;
            }

            routeValues[filter.PropertyName] = filter.Value;
            routeValues[$"{filter.PropertyName}_Operator"] = filter.Operator.ToString();
        }

        return routeValues;
    }

    public static GridRequest GetFromRequest(HttpRequest request)
    {
        var gridRequest = new GridRequest();

        if (int.TryParse(request.Query["page"], out int page))
        {
            gridRequest.Page = page;
        }
        if (int.TryParse(request.Query["pageSize"], out int pageSize))
        {
            gridRequest.PageSize = pageSize;
        }

        if (bool.TryParse(request.Query["filtersCollapsed"], out bool collapsed))
        {
            gridRequest.FiltersCollapsed = collapsed;
        }

        gridRequest.SortOrder = request.Query["sortOrder"];

        foreach (var item in request.Query)
        {
            if (item.Key == "page" || 
                item.Key == "pageSize" ||
                item.Key == "sortOrder" || 
                item.Key == "filtersCollapsed" ||
                item.Key.EndsWith("_Operator"))
            {
                continue;
            }

            string propertyName = item.Key;
            string value = item.Value!;
            string operatorKey = $"{propertyName}_Operator";

            FilterOperator filterOperator = FilterOperator.Equal;

            if (Enum.TryParse(request.Query[operatorKey], out FilterOperator parsedOperator))
            {
                filterOperator = parsedOperator;
            }

            string propertyPath;

            bool from = propertyName.EndsWith("_From");
            bool to = propertyName.EndsWith("_To");

            if (from || to)
            {
                filterOperator = from ? FilterOperator.GreaterThanOrEqual : FilterOperator.LessThanOrEqual;

                int index = propertyName.LastIndexOf('_');
                propertyPath = propertyName.Substring(0, index);

                if (propertyPath.Contains('_'))
                {
                    propertyPath = propertyPath.Replace('_', '.');
                }
            }
            else
            {
                propertyPath = propertyName.Contains('_') ? propertyName.Replace('_', '.') : propertyName;
            }

            gridRequest.Filters.Add(new FilterItem
            {
                PropertyName = propertyName,
                PropertyPath = propertyPath,
                Value = value,
                Operator = filterOperator
            });
        }

        return gridRequest;
    }
}
