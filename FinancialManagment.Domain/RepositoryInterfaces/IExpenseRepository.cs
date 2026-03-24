using FinancialManagment.Domain.Entities;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IExpenseRepository
{
    Task<(IReadOnlyList<Expense>, int, decimal)> GetAllAsync(
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

}
