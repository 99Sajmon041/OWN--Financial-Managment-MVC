using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryIntrerfaces;
using FinancialManagment.Infrastructure.Database;
using FinancialManagment.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class IncomeCategoryRepository(FinancialManagmentDbContext context) : IIncomeCategoryRepository
{
    public void Add(IncomeCategory category)
    {
        context.IncomeCategories.Add(category);
    }

    public async Task<(IReadOnlyList<IncomeCategory>, int)> GetAllAsync(string userId, PagedRequest request, CancellationToken ct)
    {
        var incomeCategories = context.IncomeCategories
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            incomeCategories = incomeCategories.Where(x => x.Name.Contains(request.Search));
        }

        var totalItemsCount = await incomeCategories.CountAsync(ct);

        incomeCategories = (request.SortBy) switch
        {
            "IsActive" => request.Desc 
                ? incomeCategories.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.Id)
                : incomeCategories.OrderBy(x => x.IsActive).ThenBy(x => x.Id),

            "Name" => request.Desc 
                ? incomeCategories.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id)
                : incomeCategories.OrderBy(x => x.Name).ThenBy(x => x.Id),

            _ => request.Desc
                ? incomeCategories.OrderByDescending(x => x.Id) 
                : incomeCategories.OrderBy(x => x.Id)
        };

        var items = await incomeCategories
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalItemsCount);
    }

    public async Task<IncomeCategory?> GetByIdAsync(int id, string userId, CancellationToken ct)
    {
        return await context.IncomeCategories.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.Id == id, ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, string userId, CancellationToken ct)
    {
        return await context.IncomeCategories.AnyAsync(x => x.ApplicationUserId == userId && x.Name == name, ct);
    }

    public async Task<bool> ExistsByNameWithDifferentIdAsync(string name, int id, string userId, CancellationToken ct)
    {
        return await context.IncomeCategories.AnyAsync(x => x.ApplicationUserId == userId && x.Name == name && x.Id != id, ct);
    }
}