using FinancialManagment.Shared.Grid.Filtering;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

    public static List<FilterOptionItem> GetMethods()
    {
        return
        [
            new FilterOptionItem { Text = "GET", Value = "GET" },
            new FilterOptionItem { Text = "POST", Value = "POST" }
        ];
    }

    public static List<FilterOptionItem> GetStatusCodes()
    {
        return
        [
            new FilterOptionItem { Text = "200", Value = "200" },
            new FilterOptionItem { Text = "201", Value = "201" },
            new FilterOptionItem { Text = "204", Value = "204" },
            new FilterOptionItem { Text = "301", Value = "301" },
            new FilterOptionItem { Text = "302", Value = "302" },
            new FilterOptionItem { Text = "400", Value = "400" },
            new FilterOptionItem { Text = "401", Value = "401" },
            new FilterOptionItem { Text = "403", Value = "403" },
            new FilterOptionItem { Text = "404", Value = "404" },
            new FilterOptionItem { Text = "405", Value = "405" },
            new FilterOptionItem { Text = "422", Value = "422" },
            new FilterOptionItem { Text = "500", Value = "500" }
        ];
    }

    public static List<FilterOptionItem> GetEndpointPaths(Assembly assembly)
    {
        List<string> paths = assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(Controller).IsAssignableFrom(type) && type.Name.EndsWith("Controller"))
            .SelectMany(controllerType =>
            {
                string controllerName = controllerType.Name.Replace("Controller", string.Empty);

                return controllerType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(method => !method.IsSpecialName && method.GetCustomAttribute<NonActionAttribute>() == null)
                    .Select(method => $"{controllerName}/{method.Name}");
            })
            .Distinct()
            .OrderBy(path => path)
            .ToList();

        return paths.Select(path => new FilterOptionItem
        {
            Text = path,
            Value = path
        })
        .ToList();            
    }
}