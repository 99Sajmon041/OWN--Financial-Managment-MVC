using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Web.RouteHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class RequestMonitoringController(IRequestMonitoringLogService requestMonitoringService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(DateTime? date, CancellationToken ct)
    {
        DateOnly dateOnly = DateOnly.FromDateTime(date ?? DateTime.Now);

        GridRequest gridRequest = GridRequestBuilder.GetFromRequest(Request);

        var result = await requestMonitoringService.ReadLogsByDateAsync(gridRequest, dateOnly, typeof(RequestMonitoringController).Assembly, ct);

        return View(result);
    }
}
