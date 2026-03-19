using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.Income;

public sealed class IncomeUpsertViewModel
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Člen domácnosti je povinný.")]
    public int HouseholdMemberId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Kategorie příjmu je povinná.")]
    public int IncomeCategoryId { get; set; }

    [Required(ErrorMessage = "Částka je povinná.")]
    [Range(0, 5000000, ErrorMessage = "Rozmezí částky příjmu je 0 - 5 000 000")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Datum příjmu je povinné.")]
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public List<SelectListItem> HouseholdMembers { get; set; } = [];
    public List<SelectListItem> IncomeCategories { get; set; } = [];
}
