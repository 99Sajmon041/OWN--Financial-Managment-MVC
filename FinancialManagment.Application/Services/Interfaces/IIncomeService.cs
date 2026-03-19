using FinancialManagment.Application.Models.Income;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IIncomeService
{
    Task<IncomeIndexViewModel> GetIndexAsync(
    PagedRequest request,
    int? householdMemberId,
    int? incomeCategoryId,
    DateTime? from,
    DateTime? to,
    CancellationToken ct);

    Task DeleteAsync(int id, CancellationToken ct);
    Task AddAsync(IncomeUpsertViewModel model, CancellationToken ct);
}
