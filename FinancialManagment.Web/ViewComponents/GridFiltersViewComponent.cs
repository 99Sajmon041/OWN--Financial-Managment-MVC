using FinancialManagment.Shared.Grid.Filtering;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.ViewComponents;

public class GridFiltersViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GridFiltersComponentModel model)
    {
        var definitions = FilterDefinitionFactory.Create(model.ModelType, model.Filters);

        if (model.CustomFilters.Count > 0)
        {
            foreach (var customFilter in model.CustomFilters)
            {
                var existingFilter = definitions.FirstOrDefault(x => x.PropertyName == customFilter.PropertyName);

                if (existingFilter is not null)
                {
                    definitions.Remove(existingFilter);
                }

                definitions.Add(customFilter);
            }
        }

        definitions = definitions
            .OrderBy(x => x.GroupName)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.PropertyName)
            .ToList();

        var vm = new GridFiltersViewModel
        {
            Fields = definitions,
            Action = model.Action,
            Controller = model.Controller,
            PageSize = model.PageSize,
            SortOrder = model.SortOrder,
            FiltersCollapsed = model.FiltersCollapsed
        };

        return View(vm);
    }
}