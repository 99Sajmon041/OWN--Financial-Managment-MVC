using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.HouseholdMember;
using FinancialManagment.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FinancialManagment.Shared.Grid.Common;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class HouseholdMemberController(IHouseholdMemberService householdMemberService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var result = await householdMemberService.GetAllAsync(ct);

        return View(result);
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View(new HouseholdMemberUpsertViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HouseholdMemberUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await householdMemberService.AddAsync(model, ct);
            TempData["Success"] = "Člen domácnosti byl úspěšně přidán.";
            return RedirectToAction(nameof(Index));
        }
        catch(ConflictException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var result = await householdMemberService.GetByIdAsync(id, ct);
        return View(result);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, HouseholdMemberUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await householdMemberService.UpdateAsync(id, model, ct);
            TempData["Success"] = "Člen domácnosti byl úspěšně upraven.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ConflictException or DomainException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, CancellationToken ct)
    {
        try
        {
            await householdMemberService.ChangeStatusAsync(id, ct);
            TempData["Success"] = "Stav člena domácnosti byl úspěšně upraven.";
            return RedirectToAction(nameof(Index));
        }
        catch(DomainException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpGet]
    public async Task<IActionResult> Grid(CancellationToken ct)
    {
        var query = Request.Query;

        var gridRequest = new GridRequest();

        if (int.TryParse(query["page"], out int page))
        {
            gridRequest.Page = page;
        }
        if (int.TryParse(query["pageSize"], out int pageSize))
        {
            gridRequest.PageSize = pageSize;
        }

        gridRequest.SortOrder = query["sortOrder"];

        foreach (var item in query)
        {
            if (item.Key == "page" || item.Key == "pageSize" || item.Key == "sortOrder")
            {
                continue;
            }

            gridRequest.Filters[item.Key] = item.Value!;
        }

        var result = await householdMemberService.GetGridAsync(gridRequest, ct);

        return View(result);
    }
}
