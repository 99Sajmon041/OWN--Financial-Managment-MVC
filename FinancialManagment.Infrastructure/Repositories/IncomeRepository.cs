using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using FinancialManagment.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class IncomeRepository(FinancialManagementDbContext context) : IIncomeRepository
{
    public async Task<(IReadOnlyList<Income>, int, decimal)> GetAllAsync(
        PagedRequest request, 
        int? householdMemberId,
        int? incomeCategoryId,
        string userId,
        DateTime from, 
        DateTime to, 
        CancellationToken ct)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Include(x => x.HouseholdMember)
            .ThenInclude(x => x.ApplicationUser)
            .Include(x => x.IncomeCategory)
            .Where(x => x.HouseholdMember.ApplicationUserId == userId && x.Date >= from && x.Date <= to);

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
        var totalIncomeSum = await query.SumAsync(x => x.Amount, ct);

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
            ? query.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id)
            : query.OrderBy(x => x.Date).ThenBy(x => x.Id),

            _ => request.Desc
            ? query.OrderByDescending(x => x.Date)
            : query.OrderBy(x => x.Date)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalItemsCount, totalIncomeSum);
    }

    async Task<Income?> IIncomeRepository.GetByIdAsync(int id, string userId, CancellationToken ct)
    {
        return await context
            .Incomes
            .Include(x => x.HouseholdMember)
            .Include(x => x.IncomeCategory)
            .FirstOrDefaultAsync(x => x.Id == id && x.HouseholdMember.ApplicationUserId == userId, ct);
    }

    void IIncomeRepository.Add(Income income)
    {
        context.Add(income);
    }

    void IIncomeRepository.Delete(Income income)
    {
        context.Remove(income);
    }

    public async Task<List<Income>> GetForJSStatisticsAsync(
        List<int> incomeCategoriesId,
        List<int> householdMembersId,
        int year,
        int month,
        string userId,
        CancellationToken ct)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Include(x => x.IncomeCategory)
            .Include(x => x.HouseholdMember)
            .Where(x => x.HouseholdMember.ApplicationUserId == userId);

        if (incomeCategoriesId.Count != 0)
            query = query.Where(x => incomeCategoriesId.Contains(x.IncomeCategoryId));

        if (householdMembersId.Count != 0)
            query = query.Where(x => householdMembersId.Contains(x.HouseholdMemberId));

        if (month == 0)
            query = query.Where(x => x.Date.Year == year);
        else
            query = query.Where(x => x.Date.Year == year && x.Date.Month == month);

        return await query.ToListAsync(ct);
    }

    public async Task<List<Income>> GetForStatisticsAsync(
    int incomeCategoryId,
    int householdMemberId,
    int year,
    int month,
    string userId,
    CancellationToken ct)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Include(x => x.IncomeCategory)
            .Include(x => x.HouseholdMember)
            .Where(x => x.HouseholdMember.ApplicationUserId == userId);

        if (incomeCategoryId != 0)
            query = query.Where(x => x.IncomeCategoryId == incomeCategoryId);

        if (householdMemberId != 0)
            query = query.Where(x => x.HouseholdMemberId == householdMemberId);

        if (month == 0)
            query = query.Where(x => x.Date.Year == year);
        else
            query = query.Where(x => x.Date.Year == year && x.Date.Month == month);

        return await query.ToListAsync(ct);
    }

    public async Task<decimal> GetTotalToDateAsync(
        List<int>? incomeCategoriesId,
        List<int>? householdMembersId,
        DateTime periodStart,
        string userId,
        CancellationToken ct)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Where(x => x.HouseholdMember.ApplicationUserId == userId && x.Date < periodStart);

        if (incomeCategoriesId is not null && incomeCategoriesId.Count > 0)
            query = query.Where(x => incomeCategoriesId.Contains(x.IncomeCategoryId));

        if (householdMembersId is not null && householdMembersId.Count > 0)
            query = query.Where(x => householdMembersId.Contains(x.HouseholdMemberId));

        var result = await query.SumAsync(x => (decimal?)x.Amount, ct);

        return result ?? 0;
    }
}
