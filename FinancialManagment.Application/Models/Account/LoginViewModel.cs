using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.Account;

public sealed class LoginViewModel
{
    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "E-mail je povinný.")]
    [EmailAddress(ErrorMessage = "Zadejte e-mail ve správném formátu.")]
    public string Email { get; set; } = default!;

    [Display(Name = "Heslo")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Heslo je povinné.")]
    public string Password { get; set; } = default!;
}