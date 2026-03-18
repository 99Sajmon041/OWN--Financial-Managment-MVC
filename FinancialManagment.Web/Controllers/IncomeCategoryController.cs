using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Pagination;
using FinancialManagment.Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class IncomeCategoryController(IIncomeCategoryService incomeCategoryService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(PagedRequest request, bool? isActive, CancellationToken ct)
    {
        var model = new IncomeCategoryIndexViewModel
        {
            Result = await incomeCategoryService.GetAllAsync(request, isActive, ct),
            SortOptions = OptionsBuilder.GetCategoryOptions()
        };

        return View(model);
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