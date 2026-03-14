using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Web.Utilities;

public static class OptionsBuilder
{
    public static List<SelectListItem> GetOptionsForIncomeCategory()
    {
        return
        [
            new SelectListItem { Text = "Stav", Value = "IsActive" },
            new SelectListItem { Text = "Jméno", Value = "Name" }
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
} 
