using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Grid.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class RequestMonitoringController(IRequestMonitoringLogService requestMonitoringService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(GridRequest gridRequest, DateTime? date, CancellationToken ct)
    {
        DateOnly dateOnly = DateOnly.FromDateTime(date ?? DateTime.UtcNow);

        var result = await requestMonitoringService.ReadLogsByDateAsync(gridRequest, dateOnly, typeof(RequestMonitoringController).Assembly, ct);

        return View(result);
    }
}
