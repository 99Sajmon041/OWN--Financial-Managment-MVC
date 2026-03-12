using Microsoft.AspNetCore.Identity;

namespace FinancialManagment.Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public ICollection<IncomeCategory> IncomeCategories { get; set; } = [];
    public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = [];
    public ICollection<Income> Incomes { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
}
