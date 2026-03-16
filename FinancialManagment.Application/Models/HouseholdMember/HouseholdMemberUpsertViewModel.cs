using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.HouseholdMember;

public sealed class HouseholdMemberUpsertViewModel
{
    public int Id { get; set; }

    [Display(Name = "Přezdívka")]
    [Required(ErrorMessage = "Přezdívka je povinná.")]
    [StringLength(50, ErrorMessage = "Přezdívka musí mít rozmezí znaků 4 - 50.", MinimumLength = 4)]
    public string Nickname { get; set; } = default!;
}
