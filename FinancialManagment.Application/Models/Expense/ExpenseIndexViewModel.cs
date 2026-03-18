using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Application.Models.Expense;

public sealed class ExpenseIndexViewModel
{
    public PagedResult<ExpenseListItemViewModel> Result { get; set; } = default!;
    public List<SelectListItem> SortOptions { get; set; } = [];
}
