using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using FinancialManagment.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class ExpenseRepository(FinancialManagementDbContext context) : IExpenseRepository
{
    public async Task<(IReadOnlyList<Expense>, int, decimal)> GetAllAsync(
        PagedRequest request,
        int? householdMemberId,
        int? expenseCategoryId,
        string userId,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Include(x => x.HouseholdMember)
            .ThenInclude(x => x.ApplicationUser)
            .Include(x => x.ExpenseCategory)
            .Where(x => x.HouseholdMember.ApplicationUserId == userId && x.Date >= from && x.Date <= to);

        if (householdMemberId is not null)
            query = query.Where(x => x.HouseholdMemberId == householdMemberId);

        if (expenseCategoryId is not null)
            query = query.Where(x => x.ExpenseCategoryId == expenseCategoryId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
            x.ExpenseCategory.Name.Contains(request.Search) ||
            x.HouseholdMember.Nickname.Contains(request.Search) ||
            (x.Description != null && x.Description.Contains(request.Search)));
        }

        var totalExpenseSum = await query.SumAsync(x => x.Amount, ct);
        var totalItemsCount = await query.CountAsync(ct);

        query = (request.SortBy) switch
        {
            "HouseholdMemberName" => request.Desc
            ? query.OrderByDescending(x => x.HouseholdMember.Nickname).ThenByDescending(x => x.Date)
            : query.OrderBy(x => x.HouseholdMember.Nickname).ThenBy(x => x.Date),

            "ExpenseCategoryName" => request.Desc
            ? query.OrderByDescending(x => x.ExpenseCategory.Name).ThenByDescending(x => x.Date)
            : query.OrderBy(x => x.ExpenseCategory.Name).ThenBy(x => x.Date),

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

        return (items, totalItemsCount, totalExpenseSum);
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

    public async Task<List<Expense>> GetForStatisticsAsync(
        int expenseCategoryId,
        int householdMemberId,
        int year,
        int month,
        string userId,
        CancellationToken ct)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Where(x => x.HouseholdMember.ApplicationUserId == userId);

        if (expenseCategoryId != 0)
            query = query.Where(x => x.ExpenseCategoryId == expenseCategoryId);

        if (householdMemberId != 0)
            query = query.Where(x => x.HouseholdMemberId == householdMemberId);

        if (month == 0)
            query = query.Where(x => x.Date.Year == year);
        else
            query = query.Where(x => x.Date.Year == year && x.Date.Month == month);

        return await query.ToListAsync(ct);
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

    public async Task<List<Expense>> GetForJsStatisticsAsync(
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
