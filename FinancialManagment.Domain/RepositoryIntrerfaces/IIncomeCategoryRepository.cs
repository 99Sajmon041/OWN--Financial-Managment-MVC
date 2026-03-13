using FinancialManagment.Domain.Entities;
using FinancialManagment.Shared.Pagination;
using System.Threading;

namespace FinancialManagment.Domain.RepositoryIntrerfaces;

public interface IIncomeCategoryRepository
{
    void Add(IncomeCategory category);
    Task<(IReadOnlyList<IncomeCategory>, int)> GetAllAsync(string userId, PagedRequest request, CancellationToken ct);
    Task<IncomeCategory?> GetByIdAsync(int id, string userId, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, string userId, CancellationToken ct);
    Task<bool> ExistsByNameWithDifferentIdAsync(string name, int id, string userId, CancellationToken ct);
}
