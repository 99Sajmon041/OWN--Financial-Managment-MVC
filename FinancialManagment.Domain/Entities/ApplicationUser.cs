using FinancialManagment.Shared.Attributes;
using Microsoft.AspNetCore.Identity;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Uživatel")]
public sealed class ApplicationUser : IdentityUser
{
    [FilterOrder(1)]
    [FilterLabel("Křestní jméno")]
    public string FirstName { get; set; } = default!;

    [FilterOrder(2)]
    [FilterLabel("Příjmení")]
    public string LastName { get; set; } = default!;
    public ICollection<IncomeCategory> IncomeCategories { get; set; } = [];
    public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = [];
    public ICollection<HouseholdMember> HouseholdMembers { get; set; } = [];
}
