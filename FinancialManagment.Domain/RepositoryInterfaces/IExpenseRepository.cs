using FinancialManagment.Domain.Entities;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IExpenseRepository
{
    Task<(IReadOnlyList<Expense>, int, decimal)> GetQueryable(
        PagedRequest request,
        int? householdMemberId,
        int? expenseCategoryId,
        string userId,
        DateTime from,
        DateTime to,
        CancellationToken ct);
    Task<Expense?> GetByIdAsync(int id, string userId, CancellationToken ct);
    void Delete(Expense expense);
    void Add(Expense expense);
    Task<List<Expense>> GetForJsStatisticsAsync(
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
