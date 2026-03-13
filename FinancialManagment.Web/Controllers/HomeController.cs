using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NotFoundPage()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
