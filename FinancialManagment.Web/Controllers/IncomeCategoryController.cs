using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Web.RouteHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class IncomeCategoryController(IIncomeCategoryService incomeCategoryService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        GridRequest gridRequest = GridRequestBuilder.GetFromRequest(Request);

        var result = await incomeCategoryService.GetAllAsync(gridRequest, ct);
        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new IncomeCategoryUpsertViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IncomeCategoryUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await incomeCategoryService.AddAsync(model, ct);

            TempData["Success"] = "Kategorie příjmů byla úspěšně přidána.";
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
        await incomeCategoryService.ChangeStatusAsync(id, ct);
        TempData["Success"] = "Stav kategorie příjmu byla úspěšně změněna.";

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var result = await incomeCategoryService.GetByIdAsync(id, ct);
        return View(result);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, IncomeCategoryUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await incomeCategoryService.UpdateAsync(id, model, ct);
            TempData["Success"] = "Kategorie příjmu byla úspěšně upravena.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is DomainException or ConflictException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}