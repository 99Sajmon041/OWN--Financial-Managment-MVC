using FinancialManagment.Domain.Entities;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IIncomeRepository
{
    IQueryable<Income> GetQueryable(string userId);
    Task<Income?> GetByIdAsync(int id, string userId, CancellationToken ct);
    void Delete(Income income);
    void Add(Income income);
    Task<List<Income>> GetForStatisticsAsync(
    List<int>? incomeCategoriesId,
    List<int>? householdMembersId,
    int year,
    int month,
    string userId,
    CancellationToken ct);

    Task<decimal> GetTotalToDateAsync(
        List<int>? incomeCategoriesId,
        List<int>? householdMembersId,
        DateTime periodStart,
        string userId,
        CancellationToken ct);
}