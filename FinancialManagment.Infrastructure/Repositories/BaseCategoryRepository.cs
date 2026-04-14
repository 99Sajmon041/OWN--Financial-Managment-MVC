using FinancialManagment.Domain.EntityInterface;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public abstract class BaseCategoryRepository<T>(FinancialManagementDbContext context) : IBaseCategoryRepository<T> where T : BaseCategory
{
    private readonly DbSet<T> db = context.Set<T>();

    public IQueryable<T> GetQueryable(string userId)
    {
        return db.AsNoTracking()
            .Where(x => x.ApplicationUserId == userId);
    }

    public void Add(T entity)
    {
        db.Add(entity);
    }

    public async Task<bool> ExistsAnyActiveAsync(string userId, CancellationToken ct)
    {
        return await db.AnyAsync(x => x.ApplicationUserId == userId && x.IsActive, ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, string userId, CancellationToken ct)
    {
        return await db.AnyAsync(x => x.ApplicationUserId == userId && x.Name == name, ct);
    }

    public async Task<bool> ExistsByNameWithDifferentIdAsync(string name, int id, string userId, CancellationToken ct)
    {
        return await db.AnyAsync(x => x.ApplicationUserId == userId && x.Name == name && x.Id != id, ct);
    }

    public async Task<List<T>> GetAllActiveAsync(string userId, CancellationToken ct)
    {
        return await db.AsNoTracking().Where(x => x.ApplicationUserId == userId && x.IsActive).ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(int id, string userId, CancellationToken ct)
    {
        return await db.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.Id == id, ct);
    }

    public async Task<bool> BelongsToUserAndIsActiveAsync(int id, string userId, CancellationToken ct)
    {
        return await db.AnyAsync(x => x.ApplicationUserId == userId && x.Id == id && x.IsActive, ct);
    }

    public async Task<IReadOnlyList<T>> GetAllCategoriesAsync(string userId, CancellationToken ct)
    {
        return await db.AsNoTracking()
            .Where(x => x.ApplicationUserId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }
}
