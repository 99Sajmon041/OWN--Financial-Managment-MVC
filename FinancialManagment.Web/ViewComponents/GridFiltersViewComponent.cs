using FinancialManagment.Shared.Grid;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.ViewComponents;

public class GridFiltersViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        Type modelType,
        Dictionary<string, string> filters,
        string action,
        string controller,
        int pageSize,
        string? sortOrder)
    {
        var fields = FilterDefinitionFactory.Create(modelType, filters);

        var vm = new GridFiltersViewModel
        {
            Fields = fields,
            Action = action,
            Controller = controller,
            PageSize = pageSize,
            SortOrder = sortOrder
        };

        return View(vm);
    }
}
