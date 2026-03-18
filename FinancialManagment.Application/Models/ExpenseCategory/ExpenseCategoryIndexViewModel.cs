using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.ExpenseCategory;

public sealed class ExpenseCategoryIndexViewModel
{
    public PagedResult<ExpenseCategoryViewModel> Result { get; set; } = default!;
    public List<SelectListItem> SortOptions { get; set; } = [];
}
