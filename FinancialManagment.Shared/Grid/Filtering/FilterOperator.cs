using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Shared.Grid.Filtering;

public enum FilterOperator
{
    [Display(Name = "Rovná se")]
    Equal = 0,

    [Display(Name = "Nerovná se")]
    NotEqual = 1,

    [Display(Name = "Obsahuje")]
    Contains = 2,

    [Display(Name = "Začíná")]
    StartsWith = 3,

    [Display(Name = "Končí")]
    EndsWith = 4,

    [Display(Name = "Menší než")]
    LessThan = 5,

    [Display(Name = "Větší než")]
    GreaterThan = 6,

    [Display(Name = "Menší nebo rovno")]
    LessThanOrEqual = 7,

    [Display(Name = "Větší nebo rovno")]
    GreaterThanOrEqual = 8,

    [Display(Name = "--Vyberte--")]
    None = 9
}
