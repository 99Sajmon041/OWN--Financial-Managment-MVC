namespace FinancialManagment.Application.FilterModels;

public sealed class StatisticsJSFilterModel
{
    public List<int> IncomeCategoriesId { get; set; } = [];
    public List<int> ExpenseCategoriesId { get; set; } = [];
    public List<int> HouseholdMembersId { get; set; } = [];
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;
}