using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using FinancialManagment.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class IncomeRepository(FinancialManagmentDbContext context) : IIncomeRepository
{
    public async Task<(IReadOnlyList<Income>, int)> GetAllAsync(
        PagedRequest request, 
        int? householdMemberId,
        int? incomeCategoryId, 
        DateTime from, 
        DateTime to, 
        CancellationToken ct)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Include(x => x.HouseholdMember)
            .Include(x => x.IncomeCategory)
            .Where(x => x.Date >= from && x.Date <= to);

        if (householdMemberId is not null)
            query = query.Where(x => x.HouseholdMemberId == householdMemberId);

        if (incomeCategoryId is not null)
            query = query.Where(x => x.IncomeCategoryId == incomeCategoryId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x => 
            x.IncomeCategory.Name.Contains(request.Search) ||
            x.HouseholdMember.Nickname.Contains(request.Search) ||
            (x.Description != null && x.Description.Contains(request.Search)));
        }

        var totalItemsCount = await query.CountAsync(ct);

        query = (request.SortBy) switch
        {
            "HouseholdMemberName" => request.Desc
            ? query.OrderByDescending(x => x.HouseholdMember.Nickname).ThenByDescending(x => x.Date)
            : query.OrderBy(x => x.HouseholdMember.Nickname).ThenBy(x => x.Date),

            "IncomeCategoryName" => request.Desc
            ? query.OrderByDescending(x => x.IncomeCategory.Name).ThenByDescending(x => x.Date)
            : query.OrderBy(x => x.IncomeCategory.Name).ThenBy(x => x.Date),

            "Amount" => request.Desc
            ? query.OrderByDescending(x => x.Amount).ThenByDescending(x => x.Date)
            : query.OrderBy(x => x.Amount).ThenBy(x => x.Date),

            "Date" => request.Desc
            ? query.OrderByDescending(x => x.Date).ThenByDescending(x => x.Date)
            : query.OrderBy(x => x.Date).ThenBy(x => x.Date),

            _ => request.Desc
            ? query.OrderByDescending(x => x.Date)
            : query.OrderBy(x => x.Date)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalItemsCount);
    }

    public async Task<Income?> GetByIdAsync(int id, string userId, CancellationToken ct)
    {
        return await context.Incomes.FirstOrDefaultAsync(x => x.Id == id && x.HouseholdMember.ApplicationUserId == userId, ct);
    }

    public void Add(Income income)
    {
        context.Add(income);
    }

    public void Delete(Income income)
    {
        context.Remove(income);
    }
}
