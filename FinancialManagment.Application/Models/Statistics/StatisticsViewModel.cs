using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Application.Models.Statistics;

public sealed class StatisticsViewModel
{
    public decimal IncomeTotal { get; set; }
    public decimal ExpenseTotal { get; set; }
    public decimal Balance => IncomeTotal - ExpenseTotal;
    public List<SelectListItem> IncomeCategories { get; set; } = [];
    public List<SelectListItem> ExpenseCategories { get; set; } = [];
    public List<SelectListItem> HouseholdMembers { get; set; } = [];
    public List<SelectListItem> Months { get; set; } = [];
    public List<SelectListItem> Years { get; set; } = [];
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month; // 0 = whole year
    public bool Yearly => SelectedMonth == 0;
    public Dictionary<string, decimal> IncomeChart { get; set; } = [];
    public Dictionary<string, decimal> ExpenseChart { get; set; } = [];
    public Dictionary<string, decimal> BalanceChart { get; set; } = [];
}
