namespace FinancialManagment.Domain.Entities;

public sealed class Income
{
    public int Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
    public string ApplicationUserId { get; set; } = default!;
    public IncomeCategory IncomeCategory { get; set; } = default!;
    public int IncomeCategoryId { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
