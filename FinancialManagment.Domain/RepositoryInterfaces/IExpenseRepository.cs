using FinancialManagment.Domain.Entities;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IExpenseRepository
{
    IQueryable<Expense> GetQueryable(string userId);
    Task<Expense?> GetByIdAsync(int id, string userId, CancellationToken ct);
    void Delete(Expense expense);
    void Add(Expense expense);
    Task<List<Expense>> GetForStatisticsAsync(
        List<int>? expenseCategoriesId,
        List<int>? householdMembersId,
        int year,
        int month,
        string userId,
        CancellationToken ct);

    Task<decimal> GetTotalToDateAsync(
        List<int>? expenseCategoriesId,
        List<int>? householdMembersId,
        DateTime periodStart,
        string userId,
        CancellationToken ct);
}
