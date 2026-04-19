using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.ExpenseCategory;
using FinancialManagment.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Web.RouteHelper;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class ExpenseCategoryController(IExpenseCategoryService expenseCategoryService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        GridRequest gridRequest = GridRequestBuilder.GetFromRequest(Request);

        var result = await expenseCategoryService.GetAllAsync(gridRequest, ct);

        return View(result);
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
            Request.HttpContext.Items["HandledStatusCode"] = StatusCodes.Status409Conflict;
            Request.HttpContext.Items["HandledExceptionType"] = ex.GetType().Name;

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
