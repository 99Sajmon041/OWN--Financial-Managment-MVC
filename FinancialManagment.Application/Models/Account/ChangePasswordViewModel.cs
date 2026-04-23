using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.Account;

public sealed class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Heslo je povinný.")]
    [DataType(DataType.Password)]
    [Display(Name = "Stávající heslo.")]
    public string CurrentPassword { get; set; } = default!;

    [Required(ErrorMessage = "Heslo je povinný")]
    [StringLength(100, ErrorMessage = "Heslo musí mít 12 - 100 znaků.", MinimumLength = 12)]
    [DataType(DataType.Password)]
    [Display(Name = "Nové heslo.")]
    public string NewPassword { get; set; } = default!;

    [Required(ErrorMessage = "Heslo je povinný")]
    [Display(Name = "Potvrzení hesla.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Hesla se musí shodovat.")]
    public string ConfirmPassword { get; set; } = default!;
}
