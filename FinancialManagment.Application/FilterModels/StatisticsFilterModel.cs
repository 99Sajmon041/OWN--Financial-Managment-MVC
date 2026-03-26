using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Application.FilterModels;

public sealed class StatisticsFilterModel
{
    public List<int> IncomeCategoriesId { get; set; } = [];
    public List<int> ExpenseCategoriesId { get; set; } = [];
    public List<int> HouseholdMemberIds { get; set; } = [];
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;
    public bool Yearly => SelectedMonth == 0;
}
