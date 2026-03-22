using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.Expense;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagment.Web.Controllers
{
    [Authorize]
    public class ExpenseController(IExpenseService expenseService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(
            PagedRequest request,
            int? householdMemberId,
            int? expenseCategoryId,
            DateTime? from,
            DateTime? to,
            CancellationToken ct)
        {
            var model = await expenseService.GetIndexAsync(request, householdMemberId, expenseCategoryId, from, to, ct);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await expenseService.DeleteAsync(id, ct);

            TempData["Success"] = "Výdaj úspěšně odstraněn.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            try
            {
                var model = await expenseService.GetForCreateAsync(ct);
                return View(model);
            }
            catch (DomainException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseUpsertViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await expenseService.FillSelectOptionsAsync(model, ct);
                return View(model);
            }

            try
            {
                await expenseService.AddAsync(model, ct);

                TempData["Success"] = "Výdaj úspěšně přidán.";
                return RedirectToAction(nameof(Index));
            }
            catch (DomainException ex)
            {
                await expenseService.FillSelectOptionsAsync(model, ct);

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken ct)
        {
            var model = await expenseService.GetForUpdateAsync(id, ct);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ExpenseUpsertViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await expenseService.FillSelectOptionsAsync(model, ct);
                return View(model);
            }

            try
            {
                await expenseService.UpdateAsync(id, model, ct);

                TempData["Success"] = "Výdaj úspěšně upraven.";
                return RedirectToAction(nameof(Index));
            }
            catch (DomainException ex)
            {
                await expenseService.FillSelectOptionsAsync(model, ct);

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
