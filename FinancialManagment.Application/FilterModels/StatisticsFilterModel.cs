namespace FinancialManagment.Application.FilterModels;

public sealed class StatisticsFilterModel
{
    public int HouseholdMemberId { get; set; } = 0;
    public int IncomeCategoryId { get; set; } = 0;
    public int ExpenseCategoryId { get; set; } = 0;
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;
}
