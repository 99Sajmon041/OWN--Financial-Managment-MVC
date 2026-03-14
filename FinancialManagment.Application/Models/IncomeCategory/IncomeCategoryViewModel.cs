using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.IncomeCategory;

public sealed class IncomeCategoryViewModel
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Název")]
    public string Name { get; set; } = default!;

    [Display(Name = "Stav")]
    public bool IsActive { get; set; } = true;
}
