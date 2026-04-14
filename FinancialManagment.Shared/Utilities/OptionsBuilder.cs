using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialManagment.Shared.Utilities;


public static class OptionsBuilder
{
    private static readonly string[] MonthNames =
    [
        "Všechny / celý rok",
        "Leden",
        "Únor",
        "Březen",
        "Duben",
        "Květen", 
        "Červen", 
        "Červenec", 
        "Srpen",
        "Září", 
        "Říjen", 
        "Listopad",
        "Prosinec"
    ];


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

    public static List<SelectListItem> GetMonths(int selectedMonth)
    {
        var months = new List<SelectListItem>();

        for (int i = 0; i <= 12; i++)
        {
            months.Add(new SelectListItem
            {
                Value = i.ToString(),
                Text = MonthNames[i],
                Selected = i == selectedMonth
            });
        }

        return months;
    }

    public static List<SelectListItem> GetYears(int selectedYear)
    {
        var years = new List<SelectListItem>();

        for (int i = 26; i <= 50; i++)
        {
            years.Add(new SelectListItem
            {
                Value = (2000 + i).ToString(),
                Text = (2000 + i).ToString(),
                Selected = (2000 + i) == selectedYear
            });
        }

        return years;
    }
}