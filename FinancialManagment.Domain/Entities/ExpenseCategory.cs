using FinancialManagment.Domain.EntityInterface;
using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Kategorie výdajů")]
public sealed class ExpenseCategory : BaseCategory
{
    public ICollection<Expense> Expenses { get; set; } = [];
}