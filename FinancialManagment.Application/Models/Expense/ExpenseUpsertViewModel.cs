using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.Expense;

public sealed class ExpenseUpsertViewModel
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Člen domácnosti")]
    [Required(ErrorMessage = "Člen domácnosti je povinný.")]
    [Range(1, int.MaxValue, ErrorMessage = "Člen domácnosti je povinný.")]
    public int HouseholdMemberId { get; set; }

    [Display(Name = "Kategorie výdaje")]
    [Required(ErrorMessage = "Kategorie výdaje je povinná.")]
    [Range(1, int.MaxValue, ErrorMessage = "Kategorie výdaje je povinná.")]
    public int ExpenseCategoryId { get; set; }

    [Display(Name = "Částka")]
    [Required(ErrorMessage = "Částka je povinná.")]
    [Range(0, 5000000, ErrorMessage = "Rozmezí částky příjmu je 0 - 5 000 000")]
    public decimal Amount { get; set; }

    [Display(Name = "Datum")]
    [Required(ErrorMessage = "Datum příjmu je povinné.")]
    public DateTime Date { get; set; }

    [Display(Name = "Popis")]
    public string? Description { get; set; }

    [Display(Name = "")]
    [StringLength(200, ErrorMessage = "Počet znaků pro nahraný obrázek musí mít do 200 znaků.", MinimumLength = 4)]
    public string? ReceiptFileName { get; set; }

    public IFormFile? ReceiptFile { get; set; }
    public List<SelectListItem> HouseholdMembers { get; set; } = [];
    public List<SelectListItem> ExpenseCategories { get; set; } = [];
}
