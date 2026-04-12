using FinancialManagment.Application.Models.Income;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IIncomeService
{
    Task<(PagedResultNew<IncomeViewModel>, decimal)> GetAllAsync(GridRequest gridRequest, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task AddAsync(IncomeUpsertViewModel model, CancellationToken ct);
    Task<IncomeUpsertViewModel> GetForCreateAsync(CancellationToken ct);
    Task<IncomeUpsertViewModel> GetForUpdateAsync(int id, CancellationToken ct);
    Task FillSelectOptionsAsync(IncomeUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, IncomeUpsertViewModel model, CancellationToken ct);
}
