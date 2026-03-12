namespace FinancialManagment.Domain.Entities;

public sealed class ExpenseCategory
{
    public int Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
    public string ApplicationUserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public ICollection<Expense> Expenses { get; set; } = [];
}