using FinancialManagment.Application.Models.Expense;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IExpenseService
{
    Task<(PagedResult<ExpenseViewModel>, decimal)> GetAllAsync(GridRequest gridRequest, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task AddAsync(ExpenseUpsertViewModel model, CancellationToken ct);
    Task<ExpenseUpsertViewModel> GetForCreateAsync(CancellationToken ct);
    Task<ExpenseUpsertViewModel> GetForUpdateAsync(int id, CancellationToken ct);
    Task FillSelectOptionsAsync(ExpenseUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, ExpenseUpsertViewModel model, CancellationToken ct);
    Task<(bool, string)> DeleteImageAsync(int id, CancellationToken ct);
}
