using FinancialManagment.Domain.Entities;
using FinancialManagment.Shared.Pagination;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IIncomeRepository
{
    Task<(IReadOnlyList<Income>, int, decimal)> GetAllAsync(
        PagedRequest request,
        int? householdMemberId,
        int? incomeCategoryId,
        string userId,
        DateTime from,
        DateTime to,
        CancellationToken ct);
    Task<Income?> GetByIdAsync(int id, string userId, CancellationToken ct);
    void Delete(Income income);
    void Add(Income income);
    Task<List<Income>> GetForJSStatisticsAsync(
    List<int> incomeCategoriesId,
    List<int> householdMembersId,
    int year,
    int month,
    string userId,
    CancellationToken ct);

    Task<List<Income>> GetForStatisticsAsync(
        int incomeCategoryId,
        int householdMemberId,
        int year,
        int month,
        string userId,
        CancellationToken ct);
}