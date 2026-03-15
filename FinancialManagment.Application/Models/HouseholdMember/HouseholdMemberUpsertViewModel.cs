namespace FinancialManagment.Application.Models.HouseholdMember;

public sealed class HouseholdMemberUpsertViewModel
{
    public int Id { get; set; }
    public string Nickname { get; set; } = default!;
}
