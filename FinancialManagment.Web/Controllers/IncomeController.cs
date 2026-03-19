using FinancialManagment.Application.Models.Income;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class IncomeController(IIncomeService incomeService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(PagedRequest request, int? householdMemberId, int? incomeCategoryId, DateTime? from, DateTime? to, CancellationToken ct)
    {
        var model = await incomeService.GetIndexAsync(request,  householdMemberId, incomeCategoryId, from, to, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await incomeService.DeleteAsync(id, ct);

        TempData["Success"] = "Příjem byl úspěšně smazán.";

        return RedirectToAction(nameof(Index));
    }

    //[HttpGet]
    //public async Task<IActionResult> Create(CancellationToken ct)
    //{
    //    var model = new IncomeUpsertViewModel
    //    {
    //        Date = DateTime.Now,
    //        HouseholdMembers = [],
    //        IncomeCategories = []
    //    };

    //    return View(model);
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(IncomeUpsertViewModel model, CancellationToken ct)
    //{

    //}
}
