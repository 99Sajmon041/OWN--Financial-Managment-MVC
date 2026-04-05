using FinancialManagment.Shared.Grid.Paging;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.ViewComponents;

public class PagerViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(PagerPartialViewModel model)
    {
        return View(model);
    }
}
