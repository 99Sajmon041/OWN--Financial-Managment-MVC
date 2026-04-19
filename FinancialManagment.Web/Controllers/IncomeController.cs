using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.Income;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Web.RouteHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class IncomeController(IIncomeService incomeService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        GridRequest gridRequest = GridRequestBuilder.GetFromRequest(Request);

        var (result, totalAmount) = await incomeService.GetAllAsync(gridRequest, ct);

        ViewBag.TotalAmount = totalAmount;
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await incomeService.DeleteAsync(id, ct);

        TempData["Success"] = "Příjem byl úspěšně smazán.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        try
        {
            var model = await incomeService.GetForCreateAsync(ct);
            return View(model);
        }
        catch (DomainException ex)
        {
            Request.HttpContext.Items["HandledStatusCode"] = StatusCodes.Status400BadRequest;
            Request.HttpContext.Items["HandledExceptionType"] = ex.GetType().Name;

            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IncomeUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await incomeService.FillSelectOptionsAsync(model, ct);
            return View(model);
        }

        try
        {
            await incomeService.AddAsync(model, ct);

            TempData["Success"] = "Příjem úspěšně přidán.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            await incomeService.FillSelectOptionsAsync(model, ct);

            Request.HttpContext.Items["HandledStatusCode"] = StatusCodes.Status400BadRequest;
            Request.HttpContext.Items["HandledExceptionType"] = ex.GetType().Name;

            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var model = await incomeService.GetForUpdateAsync(id, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, IncomeUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await incomeService.FillSelectOptionsAsync(model, ct);
            return View(model);
        }

        try
        {
            await incomeService.UpdateAsync(id, model, ct);

            TempData["Success"] = "Příjem úspěšně upraven.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            await incomeService.FillSelectOptionsAsync(model, ct);

            Request.HttpContext.Items["HandledStatusCode"] = StatusCodes.Status400BadRequest;
            Request.HttpContext.Items["HandledExceptionType"] = ex.GetType().Name;

            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
