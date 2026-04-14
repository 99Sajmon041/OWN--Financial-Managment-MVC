using FinancialManagment.Domain.EntityInterface;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IBaseCategoryRepository<T> where T : BaseCategory
{
    IQueryable<T> GetQueryable(string userId);
    void Add(T entity);
    Task<T?> GetByIdAsync(int id, string userId, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, string userId, CancellationToken ct);
    Task<bool> ExistsByNameWithDifferentIdAsync(string name, int id, string userId, CancellationToken ct);
    Task<List<T>> GetAllActiveAsync(string userId, CancellationToken ct);
    Task<bool> ExistsAnyActiveAsync(string userId, CancellationToken ct);
    Task<bool> BelongsToUserAndIsActiveAsync(int id, string userId, CancellationToken ct);
    Task<IReadOnlyList<T>> GetAllCategoriesAsync(string userId, CancellationToken ct);
}
