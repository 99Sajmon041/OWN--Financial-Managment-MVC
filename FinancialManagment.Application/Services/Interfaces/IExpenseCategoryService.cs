using FinancialManagment.Application.Models.ExpenseCategory;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IExpenseCategoryService
{
    Task<PagedResult<ExpenseCategoryViewModel>> GetAllAsync(GridRequest gridRequest, CancellationToken ct);
    Task AddAsync(ExpenseCategoryUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, ExpenseCategoryUpsertViewModel model, CancellationToken ct);
    Task ChangeStatusAsync(int id, CancellationToken ct);
    Task<ExpenseCategoryUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct);
}
