using FinancialManagment.Application.Export;
using FinancialManagment.Application.Models.Monitoring;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Web.RouteHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class RequestMonitoringController(IRequestMonitoringLogService requestMonitoringService, ICsvExportService csvExportService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(DateTime? selectedDate, CancellationToken ct, bool export = false)
    {
        DateOnly dateOnly = DateOnly.FromDateTime(selectedDate ?? DateTime.Now);

        GridRequest gridRequest = GridRequestBuilder.GetFromRequest(Request);

        var result = await requestMonitoringService.ReadLogsByDateAsync(gridRequest, dateOnly, typeof(RequestMonitoringController).Assembly, ct);

        if (export)
        {
            var columns = CsvColumnDefinitionBuilder.Build<RequestMonitoringLogItem>();

            var items = await requestMonitoringService.GetAllReadLogsByDateAsync(dateOnly, ct);

            byte[] fileBytes = csvExportService.ExportToCsv(items, columns);

            string fileName = $"request-monitoring-{dateOnly:yyyy-MM-dd}.csv";

            return File(fileBytes, "text/csv", fileName);
        }

        return View(result);
    }
}
