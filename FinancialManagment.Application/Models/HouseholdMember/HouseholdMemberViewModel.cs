namespace FinancialManagment.Application.Models.HouseholdMember;

public sealed class HouseholdMemberViewModel
{
    public int Id { get; set; }
    public string Nickname { get; set; } = default!;
    public bool IsActive { get; set; }
}
