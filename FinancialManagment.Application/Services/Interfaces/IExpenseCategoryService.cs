using FinancialManagment.Application.Models.ExpenseCategory;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IExpenseCategoryService
{
    Task AddAsync(ExpenseCategoryUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, ExpenseCategoryUpsertViewModel model, CancellationToken ct);
    Task ChangeStatusAsync(int id, CancellationToken ct);
    Task<ExpenseCategoryUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct);
    Task<PagedResult<ExpenseCategoryViewModel>> GetAllAsync(PagedRequest pagedRequest, bool? isActive, CancellationToken ct);
}
