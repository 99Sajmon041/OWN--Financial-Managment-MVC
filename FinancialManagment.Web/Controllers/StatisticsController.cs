using FinancialManagment.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers
{
    [Authorize]
    public class StatisticsController(IStatisticsService statisticsService) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
