using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Příjem")]
public sealed class Income
{
    public int Id { get; set; }
    public HouseholdMember HouseholdMember { get; set; } = default!;
    public int HouseholdMemberId { get; set; }
    public IncomeCategory IncomeCategory { get; set; } = default!;
    public int IncomeCategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
