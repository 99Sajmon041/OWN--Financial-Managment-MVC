using FinancialManagment.Domain.Entities;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IIncomeRepository
{
    Task<(IReadOnlyList<Income>, int)> GetAllAsync(
        PagedRequest request,
        int? householdMemberId,
        int? incomeCategoryId,
        DateTime from,
        DateTime to,
        CancellationToken ct);

    Task<Income?> GetByIdAsync(int id, string userId, CancellationToken ct);
    void Delete(Income income);
    void Add(Income income);
}
