using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class HouseholdMemberRepository(FinancialManagementDbContext context) : IHouseholdMemberRepository
{
    public void Add(HouseholdMember householdMember)
    {
        context.Add(householdMember);
    }

    public async Task<List<HouseholdMember>> GetAllAsync(string userId, CancellationToken ct)
    {
        return await context
            .HouseholdMembers
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId)
            .OrderBy(x => x.Nickname)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string userId, string nickName, CancellationToken ct)
    {
        return await context.HouseholdMembers.AnyAsync(x => x.ApplicationUserId == userId && x.Nickname == nickName, ct);
    }

    public async Task<HouseholdMember?> GetByIdAsync(int id, string userId, CancellationToken ct)
    {
        return await context.HouseholdMembers.FirstOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == userId, ct);
    }

    public async Task<bool> ExistsByNameWithDifferentIdAsync(string nickname, int id, string userId, CancellationToken ct)
    {
        return await context.HouseholdMembers.AnyAsync(x => x.ApplicationUserId == userId && x.Nickname == nickname && x.Id != id, ct);
    }

    public async Task<int> GetCountOfMembersAsync(string userId, CancellationToken ct)
    {
        return await context
            .HouseholdMembers
            .Where(x => x.ApplicationUserId == userId)
            .CountAsync(ct);
    }

    public async Task<List<HouseholdMember>> GetAllActiveAsync(string userId, CancellationToken ct)
    {
        return await context.HouseholdMembers
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAnyActiveAsync(string userId, CancellationToken ct)
    {
        return await context.HouseholdMembers.AnyAsync(x => x.ApplicationUserId == userId && x.IsActive, ct);
    }

    public async Task<bool> BelongsToUserAndIsActiveAsync(int id, string userId, CancellationToken ct)
    {
        return await context.HouseholdMembers.AnyAsync(x => x.Id == id && x.ApplicationUserId == userId && x.IsActive, ct);
    }


    /// <summary>
    /// Method for retreiving queryable list of household members for a specific user.
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <returns>Returns query list of household members</returns>
    public IQueryable<HouseholdMember> GetQueryable(string userId)
    {
        return context.HouseholdMembers
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId);
    }
}
