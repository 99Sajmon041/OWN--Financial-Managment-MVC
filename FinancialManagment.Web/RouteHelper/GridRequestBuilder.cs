using FinancialManagment.Shared.Grid.Common;

namespace FinancialManagment.Web.RouteHelper;

public static class GridRequestBuilder
{
    public static Dictionary<string, string> GetRouteValues(GridRequest gridRequest, string sortValue)
    {
        Dictionary<string, string> routeValues = new(gridRequest.Filters)
        {
            ["page"] = "1",
            ["pageSize"] = gridRequest.PageSize.ToString(),
            ["filtersCollapsed"] = gridRequest.FiltersCollapsed.ToString().ToLower(),
            ["sortOrder"] = sortValue
        };

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
            if (item.Key == "page" || item.Key == "pageSize" || item.Key == "sortOrder" || item.Key == "filtersCollapsed")
            {
                continue;
            }

            gridRequest.Filters[item.Key] = item.Value!;
        }

        return gridRequest;
    }
}
