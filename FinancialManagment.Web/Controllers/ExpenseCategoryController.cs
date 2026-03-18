using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.ExpenseCategory;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Pagination;
using FinancialManagment.Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class ExpenseCategoryController(IExpenseCategoryService expenseCategoryService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(PagedRequest request, bool? isActive, CancellationToken ct)
    {
        var model = new ExpenseCategoryIndexViewModel
        {
            Result = await expenseCategoryService.GetAllAsync(request, isActive, ct),
            SortOptions = OptionsBuilder.GetCategoryOptions()
        };

        return View(model);
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View(new ExpenseCategoryUpsertViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseCategoryUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await expenseCategoryService.AddAsync(model, ct);

            TempData["Success"] = "Kategorie výdajů byla úspěšně přidána.";
            return RedirectToAction(nameof(Index));
        }
        catch (ConflictException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, CancellationToken ct)
    {
        await expenseCategoryService.ChangeStatusAsync(id, ct);
        TempData["Success"] = "Stav výdajové kategorie byl úspěšně změněn.";

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var result = await expenseCategoryService.GetByIdAsync(id, ct);
        return View(result);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, ExpenseCategoryUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await expenseCategoryService.UpdateAsync(id, model, ct);
            TempData["Success"] = "Kategorie výdaje byla úspěšně upravena.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is DomainException or ConflictException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
