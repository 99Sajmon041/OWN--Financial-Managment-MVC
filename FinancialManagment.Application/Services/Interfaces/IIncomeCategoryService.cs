using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IIncomeCategoryService
{
    Task<PagedResultNew<IncomeCategoryViewModel>> GetAllAsync(GridRequest gridRequest, CancellationToken ct);
    Task AddAsync(IncomeCategoryUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, IncomeCategoryUpsertViewModel model, CancellationToken ct);
    Task ChangeStatusAsync(int id, CancellationToken ct);
    Task<IncomeCategoryUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct);
}