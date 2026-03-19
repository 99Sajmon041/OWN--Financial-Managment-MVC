using FinancialManagment.Application.Models.HouseholdMember;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IHouseholdMemberService
{
    Task<List<HouseholdMemberViewModel>> GetAllAsync(CancellationToken ct);
    Task AddAsync(HouseholdMemberUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, HouseholdMemberUpsertViewModel model, CancellationToken ct);
    Task<HouseholdMemberUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct);
    Task ChangeStatusAsync(int id, CancellationToken ct);
}
