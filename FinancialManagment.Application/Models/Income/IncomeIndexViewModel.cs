using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Application.Models.Income;

public sealed class IncomeIndexViewModel
{
    public PagedResult<IncomeViewModel> Result { get; set; } = default!;
    public List<SelectListItem> SortOptions { get; set; } = [];
    public List<SelectListItem> HouseholdMemberOptions { get; set; } = [];
    public List<SelectListItem> IncomeCategoryOptions { get; set; } = [];
    public decimal IncomeSum { get; set; } = 0;
}