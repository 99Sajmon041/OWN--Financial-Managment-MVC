using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Výdaj")]
public sealed class Expense
{
    public int Id { get; set; }
    public HouseholdMember HouseholdMember { get; set; } = default!;
    public int HouseholdMemberId { get; set; }
    public ExpenseCategory ExpenseCategory { get; set; } = default!;
    public int ExpenseCategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string? ReceiptFileName { get; set; }
}
