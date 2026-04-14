using FinancialManagment.Application.Models.HouseholdMember;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IHouseholdMemberService
{
    Task<PagedResult<HouseholdMemberViewModel>> GetAllAsync(GridRequest gridRequest, CancellationToken ct);
    Task AddAsync(HouseholdMemberUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, HouseholdMemberUpsertViewModel model, CancellationToken ct);
    Task<HouseholdMemberUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct);
    Task ChangeStatusAsync(int id, CancellationToken ct);
}
