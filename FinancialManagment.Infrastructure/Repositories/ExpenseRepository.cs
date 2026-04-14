using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class ExpenseRepository(FinancialManagementDbContext context) : IExpenseRepository
{
    public IQueryable<Expense> GetQueryable(string userId)
    {
        return context
            .Expenses
            .Include(x => x.HouseholdMember)
            .Include(x => x.ExpenseCategory)
            .Where(x => x.HouseholdMember.ApplicationUserId == userId);
    }

    public async Task<Expense?> GetByIdAsync(int id, string userId, CancellationToken ct)
    {
        return await context
            .Expenses
            .Include(x => x.HouseholdMember)
            .Include(x => x.ExpenseCategory)
            .FirstOrDefaultAsync(x => x.Id == id && x.HouseholdMember.ApplicationUserId == userId, ct);
    }

    public void Add(Expense expense)
    {
        context.Add(expense);
    }

    public void Delete(Expense expense)
    {
        context.Remove(expense);
    }

    public async Task<decimal> GetTotalToDateAsync(
        List<int>? expenseCategoriesId,
        List<int>? householdMembersId,
        DateTime periodStart,
        string userId,
        CancellationToken ct)
    { 
        var query = context.Expenses
            .AsNoTracking()
            .Where(x => x.HouseholdMember.ApplicationUserId == userId && x.Date < periodStart);

        if (expenseCategoriesId is not null && expenseCategoriesId.Count > 0)
            query = query.Where(x => expenseCategoriesId.Contains(x.ExpenseCategoryId));

        if (householdMembersId is not null && householdMembersId.Count > 0)
            query = query.Where(x => householdMembersId.Contains(x.HouseholdMemberId));

        var result = await query.SumAsync(x => (decimal?)x.Amount, ct);

        return result ?? 0;
    }

    public async Task<List<Expense>> GetForStatisticsAsync(
    List<int>? expenseCategoriesId,
    List<int>? householdMembersId,
    int year,
    int month,
    string userId,
    CancellationToken ct)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Where(x => x.HouseholdMember.ApplicationUserId == userId);

        if (expenseCategoriesId is not null && expenseCategoriesId.Count > 0)
            query = query.Where(x => expenseCategoriesId.Contains(x.ExpenseCategoryId));

        if (householdMembersId is not null && householdMembersId.Count > 0)
            query = query.Where(x => householdMembersId.Contains(x.HouseholdMemberId));

        if (month == 0)
            query = query.Where(x => x.Date.Year == year);
        else
            query = query.Where(x => x.Date.Year == year && x.Date.Month == month);

        return await query.ToListAsync(ct);
    }
}
