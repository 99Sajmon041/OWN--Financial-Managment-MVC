using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.HouseholdMember;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Web.RouteHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers;

[Authorize]
public class HouseholdMemberController(IHouseholdMemberService householdMemberService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        GridRequest gridRequest = GridRequestBuilder.GetFromRequest(Request);

        var result = await householdMemberService.GetAllAsync(gridRequest, ct);

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
        catch (Exception ex) when (ex is ConflictException or DomainException)
        {
            Request.HttpContext.Items["HandledStatusCode"] = ex is ConflictException ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            Request.HttpContext.Items["HandledExceptionType"] = ex.GetType().Name;

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
            Request.HttpContext.Items["HandledStatusCode"] = ex is ConflictException ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            Request.HttpContext.Items["HandledExceptionType"] = ex.GetType().Name;

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
            Request.HttpContext.Items["HandledStatusCode"] = StatusCodes.Status400BadRequest;
            Request.HttpContext.Items["HandledExceptionType"] = ex.GetType().Name;

            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}


