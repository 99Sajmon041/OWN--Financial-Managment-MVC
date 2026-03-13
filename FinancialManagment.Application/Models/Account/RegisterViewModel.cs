using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.Account;

public sealed class RegisterViewModel
{
    [Display(Name = "Jméno")]
    [Required(ErrorMessage = "Jméno je povinné.")]
    [StringLength(200, ErrorMessage = "Zadejte jméno v rozmezí znaků: 2 - 200.", MinimumLength = 2)]
    public string FirstName { get; set; } = default!;

    [Display(Name = "Příjmení")]
    [Required(ErrorMessage = "Příjmení je povinné.")]
    [StringLength(200, ErrorMessage = "Zadejte příjmení v rozmezí znaků: 2 - 200.", MinimumLength = 2)]
    public string LastName { get; set; } = default!;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "E-mail je povinný.")]
    [EmailAddress(ErrorMessage = "Zadejte e-mail ve správném formátu.")]
    public string Email { get; set; } = default!;

    [Display(Name = "Heslo")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Heslo je povinné.")]
    public string Password { get; set; } = default!;

    [Display(Name = "Potvrzení hesla")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Potvrzení hesla je povinné.")]
    [Compare(nameof(Password), ErrorMessage = "Hesla se musí shodovat.")]
    public string ConfirmPassword { get; set; } = default!;
}
