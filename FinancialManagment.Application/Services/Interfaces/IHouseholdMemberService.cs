using FinancialManagment.Application.Models.HouseholdMember;
using FinancialManagment.Shared.Grid;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IHouseholdMemberService
{
    Task<List<HouseholdMemberViewModel>> GetAllAsync(CancellationToken ct);
    Task AddAsync(HouseholdMemberUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, HouseholdMemberUpsertViewModel model, CancellationToken ct);
    Task<HouseholdMemberUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct);
    Task ChangeStatusAsync(int id, CancellationToken ct);

    //Service method interface for grid with dynamic filtering, sorting, and pagination
    Task<PagedResultNew<HouseholdMemberViewModel>> GetGridAsync(GridRequest gridRequest, CancellationToken ct);
}
