using FinancialManagment.Domain.Entities;

namespace FinancialManagment.Domain.RepositoryInterfaces;

public interface IHouseholdMemberRepository
{
    IQueryable<HouseholdMember> GetQueryable(string userId);
    void Add(HouseholdMember householdMember);
    Task<List<HouseholdMember>> GetAllActiveAsync(string userId, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string userId, string nickName, CancellationToken ct);
    Task<HouseholdMember?> GetByIdAsync(int id, string userId, CancellationToken ct);
    Task<bool> ExistsByNameWithDifferentIdAsync(string nickname, int id, string userId, CancellationToken ct);
    Task<int> GetCountOfMembersAsync(string userId, CancellationToken ct);
    Task<bool> ExistsAnyActiveAsync(string userId, CancellationToken ct);
    Task<bool> BelongsToUserAndIsActiveAsync(int id, string userId, CancellationToken ct);
}
