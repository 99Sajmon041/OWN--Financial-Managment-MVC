using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Člen domácnosti")]
public sealed class HouseholdMember
{
    [NotFilterable]
    public int Id { get; set; }

    [NotFilterable]
    public ApplicationUser ApplicationUser { get; set; } = default!;

    [NotFilterable]
    public string ApplicationUserId { get; set; } = default!;

    [FilterLabel("Přezdívka člena")]
    [FilterOrder(1)]
    public string Nickname { get; set; } = default!;

    [FilterLabel("Aktivní člen")]
    [FilterOrder(2)]
    public bool IsActive { get; set; }

    public ICollection<Income> Incomes { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
}