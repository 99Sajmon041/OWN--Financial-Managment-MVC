using FinancialManagment.Domain.EntityInterface;

namespace FinancialManagment.Domain.Entities;

public sealed class ExpenseCategory : BaseCategory
{
    public ICollection<Expense> Expenses { get; set; } = [];
}