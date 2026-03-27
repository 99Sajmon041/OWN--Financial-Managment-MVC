namespace FinancialManagment.Application.FilterModels;

public sealed class StatisticsFilterModel
{
    public int IncomeCategoryId { get; set; }
    public int ExpenseCategoryId { get; set; }
    public int HouseholdMemberId { get; set; }
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;
}
