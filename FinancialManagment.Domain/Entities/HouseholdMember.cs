namespace FinancialManagment.Domain.Entities;

public sealed class HouseholdMember
{
    public int Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
    public string ApplicationUserId { get; set; } = default!;
    public string Nickname { get; set; } = default!;
    public bool IsActive { get; set; }
    public ICollection<Income> Incomes { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
}
