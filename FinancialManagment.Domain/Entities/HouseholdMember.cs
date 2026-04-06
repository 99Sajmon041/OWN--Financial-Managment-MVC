using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

public sealed class HouseholdMember
{
    [FilterLabel("ID")]
    [FilterOrder(3)]
    public int Id { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = default!;

    [NotFilterable]
    public string ApplicationUserId { get; set; } = default!;

    [FilterLabel("Přezdívka")]
    [FilterOrder(1)]
    public string Nickname { get; set; } = default!;

    [FilterLabel("Aktivní")]
    [FilterOrder(2)]
    public bool IsActive { get; set; }

    public ICollection<Income> Incomes { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
}