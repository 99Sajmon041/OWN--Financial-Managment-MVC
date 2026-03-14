using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IIncomeCategoryService
{
    Task AddAsync(IncomeCategoryUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, IncomeCategoryUpsertViewModel model, CancellationToken ct);
    Task ChangeStatusAsync(int id, CancellationToken ct);
    Task<IncomeCategoryUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct);
    Task<PagedResult<IncomeCategoryViewModel>> GetAllAsync(PagedRequest pagedRequest, CancellationToken ct);
}