using FinancialManagment.Application.FilterModels;
using FinancialManagment.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class StatisticsController(IStatisticsService statisticsService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index(StatisticsFilterModel filterModel, CancellationToken ct)
    {
        var model = await statisticsService.GetJsStatisticsAsync(filterModel, ct);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> IndexData([FromBody] StatisticsFilterModel filterModel, CancellationToken ct)
    {
        var model = await statisticsService.GetJsStatisticsAsync(filterModel, ct);
        return Json(model);
    }
}
