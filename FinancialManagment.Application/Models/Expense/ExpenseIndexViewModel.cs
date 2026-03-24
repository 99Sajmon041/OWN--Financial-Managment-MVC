using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Application.Models.Expense;

public sealed class ExpenseIndexViewModel
{
    public PagedResult<ExpenseViewModel> Result { get; set; } = default!;
    public List<SelectListItem> SortOptions { get; set; } = [];
    public List<SelectListItem> HouseholdMemberOptions { get; set; } = [];
    public List<SelectListItem> ExpenseCategoryOptions { get; set; } = [];
    public decimal ExpenseSum { get; set; } = 0;
}
