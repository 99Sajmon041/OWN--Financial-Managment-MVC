using FinancialManagment.Shared.Attributes;
using Microsoft.AspNetCore.Identity;

namespace FinancialManagment.Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public ICollection<IncomeCategory> IncomeCategories { get; set; } = [];
    public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = [];
    public ICollection<HouseholdMember> HouseholdMembers { get; set; } = [];
}
