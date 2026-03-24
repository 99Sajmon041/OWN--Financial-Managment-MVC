using FinancialManagment.Application.Models.Expense;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IExpenseService
{
    Task<ExpenseIndexViewModel> GetIndexAsync(
        PagedRequest request,
        int? householdMemberId,
        int? expenseCategoryId,
        DateTime? from,
        DateTime? to,
        CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task AddAsync(ExpenseUpsertViewModel model, CancellationToken ct);
    Task<ExpenseUpsertViewModel> GetForCreateAsync(CancellationToken ct);
    Task<ExpenseUpsertViewModel> GetForUpdateAsync(int id, CancellationToken ct);
    Task FillSelectOptionsAsync(ExpenseUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, ExpenseUpsertViewModel model, CancellationToken ct);
    Task<(bool, string)> DeleteImageAsync(int id, CancellationToken ct);
}
