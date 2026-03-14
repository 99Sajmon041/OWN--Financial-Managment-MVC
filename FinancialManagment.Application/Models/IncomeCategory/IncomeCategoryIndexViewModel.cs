using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.IncomeCategory;

public sealed class IncomeCategoryIndexViewModel
{
    public PagedResult<IncomeCategoryViewModel> Result { get; set; } = default!;

    [Display(Name = "Filtry")]
    public List<SelectListItem> SortOptions { get; set; } = [];
}
