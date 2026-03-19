using FinancialManagment.Domain.EntityInterface;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using FinancialManagment.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public abstract class BaseCategoryRepository<T>(FinancialManagmentDbContext context) : IBaseCategoryRepository<T> where T : BaseCategory
{
    private readonly DbSet<T> db = context.Set<T>();

    public void Add(T entity)
    {
        db.Add(entity);
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

    public async Task<(IReadOnlyList<T>, int)> GetAllAsync(string userId, bool? isActive, PagedRequest request, CancellationToken ct)
    {
        var query = db
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId);

        if (isActive is not null)
        {
            query = query.Where(x => x.IsActive == isActive);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x => x.Name.Contains(request.Search));
        }

        var totalItemsCount = await query.CountAsync(ct);

        query = (request.SortBy) switch
        {
            "IsActive" => request.Desc
                ? query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.IsActive).ThenBy(x => x.Id),

            "Name" => request.Desc
                ? query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Name).ThenBy(x => x.Id),

            _ => request.Desc
                ? query.OrderByDescending(x => x.Id)
                : query.OrderBy(x => x.Id)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalItemsCount);
    }

    public async Task<T?> GetByIdAsync(int id, string userId, CancellationToken ct)
    {
        return await db.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.Id == id, ct);
    }
}
