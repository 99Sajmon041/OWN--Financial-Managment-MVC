using FinancialManagment.Shared.Grid;
using FinancialManagment.Shared.Grid.Filtering;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.ViewComponents;

public class GridFiltersViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GridFiltersComponentModel model)
    {
        var fields = FilterDefinitionFactory.Create(model.ModelType, model.Filters);

        var vm = new GridFiltersViewModel
        {
            Fields = fields,
            Action = model.Action,
            Controller = model.Controller,
            PageSize = model.PageSize,
            SortOrder = model.SortOrder
        };

        return View(vm);
    }
}