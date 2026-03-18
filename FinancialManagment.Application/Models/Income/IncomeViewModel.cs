namespace FinancialManagment.Application.Models.Income;

public sealed class IncomeViewModel
{
    public int Id { get; set; }
    public string HouseholdMemberNickname { get; set; } = default!;
    public string IncomeCategoryName { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
