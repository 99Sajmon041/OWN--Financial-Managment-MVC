using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class IncomeRepository(FinancialManagementDbContext context) : IIncomeRepository
{
    public IQueryable<Income> GetQueryable(string userId)
    {
        return context
            .Incomes
            .Include(x => x.HouseholdMember)
            .Include(x => x.IncomeCategory)
            .Where(x => x.HouseholdMember.ApplicationUserId == userId);
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

    public async Task<List<Income>> GetForStatisticsAsync(
        List<int>? incomeCategoriesId,
        List<int>? householdMembersId,
        int year,
        int month,
        string userId,
        CancellationToken ct)
    {
        var query = context.Incomes
            .AsNoTracking()
            .Where(x => x.HouseholdMember.ApplicationUserId == userId);

        if (incomeCategoriesId is not null && incomeCategoriesId.Count > 0)
            query = query.Where(x => incomeCategoriesId.Contains(x.IncomeCategoryId));

        if (householdMembersId is not null && householdMembersId.Count > 0)
            query = query.Where(x => householdMembersId.Contains(x.HouseholdMemberId));

        if (month == 0)
            query = query.Where(x => x.Date.Year == year);
        else
            query = query.Where(x => x.Date.Year == year && x.Date.Month == month);

        return await query.ToListAsync(ct);
    }
}
