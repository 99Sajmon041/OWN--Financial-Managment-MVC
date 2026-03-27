using FinancialManagment.Application.FilterModels;
using FinancialManagment.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers
{
    [Authorize]
    public class StatisticsController(IStatisticsService statisticsService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(StatisticsFilterModel filterModel, CancellationToken ct)
        {
            var model = await statisticsService.GetStatisticsAsync(filterModel, ct);
            return View(model);
        }

        [HttpGet("js-index")]
        public async Task<IActionResult> JsIndex(StatisticsJSFilterModel filterModel, CancellationToken ct)
        {
            var model = await statisticsService.GetJSStatisticsAsync(filterModel, ct);
            return View(model);
        }
    }
}
