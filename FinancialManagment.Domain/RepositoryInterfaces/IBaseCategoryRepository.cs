using FinancialManagment.Domain.EntityInterface;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IBaseCategoryRepository<T> where T : BaseCategory
{
    void Add(T entity);
    Task<(IReadOnlyList<T>, int)> GetAllAsync(string userId, bool? isActive, PagedRequest request, CancellationToken ct);
    Task<T?> GetByIdAsync(int id, string userId, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, string userId, CancellationToken ct);
    Task<bool> ExistsByNameWithDifferentIdAsync(string name, int id, string userId, CancellationToken ct);
}
