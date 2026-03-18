using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Web.Utilities;

public static class OptionsBuilder
{
    public static List<SelectListItem> GetCategoryOptions()
    {
        return
        [
            new SelectListItem { Text = "Stav", Value = "IsActive" },
            new SelectListItem { Text = "Název", Value = "Name" }
        ];
    }

    public static List<SelectListItem> GetPageSizeOptions()
    {
        return
        [
            new SelectListItem { Value = "5", Text = "5" },
            new SelectListItem { Value = "10", Text = "10" },
            new SelectListItem { Value = "20", Text = "20" },
            new SelectListItem { Value = "50", Text = "50" }
        ];
    }

    public static List<SelectListItem> GetExpenseOrIncomeOptions(bool forExpense)
    {
        var options = new List<SelectListItem>()
        {
            new() { Text = "Člen", Value = "HouseholdMemberName" },
            new() { Text = "Částka", Value = "Amount" },
            new() { Text = "Datum", Value = "Date" }
        };

        if (forExpense)
            options.Add(new SelectListItem { Text = "Typ výdaje", Value = "ExpenseCategoryName" });
        else
            options.Add(new SelectListItem { Text = "Typ příjmu", Value = "IncomeCategoryName" });

        return options;
    }
} 
