namespace FinancialManagment.Application.Models.Expense;

public sealed class ExpenseViewModel
{
    public int Id { get; set; }
    public string HouseholdMemberNickname { get; set; } = default!;
    public string ExpenseCategoryName { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string? ReceiptFileName { get; set; }
}
