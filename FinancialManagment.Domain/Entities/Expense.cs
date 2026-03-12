namespace FinancialManagment.Domain.Entities;

public sealed class Expense
{
    public int Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
    public string ApplicationUserId { get; set; } = default!;
    public ExpenseCategory ExpenseCategory { get; set; } = default!;
    public int ExpenseCategoryId { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string? ReceiptFileName { get; set; }
}
