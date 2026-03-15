using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.IncomeCategory;

public sealed class IncomeCategoryUpsertViewModel
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Název")]
    [Required(ErrorMessage = "Název kategorie je povinný.")]
    [StringLength(200, ErrorMessage = "Název kategorie musí být v rozmezí 5 - 200 znaků.", MinimumLength = 5)]
    public string Name { get; set; } = default!;
}
