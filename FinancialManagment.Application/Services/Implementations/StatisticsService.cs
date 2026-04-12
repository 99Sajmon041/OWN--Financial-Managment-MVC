using FinancialManagment.Application.FilterModels;
using FinancialManagment.Application.Models.Statistics;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Shared.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class StatisticsService(
    ILogger<StatisticsService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IStatisticsService
{
    public async Task<StatisticsJsViewModel> GetJsStatisticsAsync(StatisticsJsFilterModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;
        var (houseHoldmembersListItems, incomeCategoriesListItems, expenseCategoriesListItems) = await GetSelectListData(userId, ct);

        List<int>? incomesCtgIds;
        List<int>? expensesCtgIds;
        List<int>? houseHoldmembersIds;

        if (model.HouseholdMembersId.Contains(0) || model.HouseholdMembersId.Count == 0)
        {
            var item = houseHoldmembersListItems.First(x => x.Value == "0");
            houseHoldmembersIds = null;
            item.Selected = true;
        }
        else
        {
            foreach (var houseHoldmember in houseHoldmembersListItems.OrderBy(x => int.Parse(x.Value)))
            {
                if (model.HouseholdMembersId.Contains(int.Parse(houseHoldmember.Value)))
                {
                    houseHoldmember.Selected = true;
                }
            }

            houseHoldmembersIds = model.HouseholdMembersId;
        }

        if (model.IncomeCategoriesId.Contains(0) || model.IncomeCategoriesId.Count == 0)
        {
            var item = incomeCategoriesListItems.First(x => x.Value == "0");
            incomesCtgIds = null;
            item.Selected = true;
        }
        else
        {
            foreach (var incomesCtg in incomeCategoriesListItems.OrderBy(x => int.Parse(x.Value)))
            {
                if (model.IncomeCategoriesId.Contains(int.Parse(incomesCtg.Value)))
                {
                    incomesCtg.Selected = true;
                }
            }

            incomesCtgIds = model.IncomeCategoriesId;
        }

        if (model.ExpenseCategoriesId.Contains(0) || model.ExpenseCategoriesId.Count == 0)
        {
            var item = expenseCategoriesListItems.First(x => x.Value == "0");
            expensesCtgIds = null;
            item.Selected = true;
        }
        else
        {
            foreach (var expenseCtg in expenseCategoriesListItems.OrderBy(x => int.Parse(x.Value)))
            {
                if (model.ExpenseCategoriesId.Contains(int.Parse(expenseCtg.Value)))
                {
                    expenseCtg.Selected = true;
                }
            }

            expensesCtgIds = model.ExpenseCategoriesId;
        }

        var incomes = await unitOfWork.IncomeRepository.GetForJsStatisticsAsync(
            incomesCtgIds,
            houseHoldmembersIds,
            model.SelectedYear,
            model.SelectedMonth,
            userId,
            ct);

        var expenses = await unitOfWork.ExpenseRepository.GetForJsStatisticsAsync(
            expensesCtgIds,
            houseHoldmembersIds,
            model.SelectedYear,
            model.SelectedMonth,
            userId,
            ct);

        var months = OptionsBuilder.GetMonths(model.SelectedMonth);
        var years = OptionsBuilder.GetYears(model.SelectedYear);

        DateTime periodStart;

        if (model.SelectedMonth == 0)
        {
            periodStart = new DateTime(model.SelectedYear, 1, 1);
        }
        else
        {
            periodStart = new DateTime(model.SelectedYear, model.SelectedMonth, 1);
        }

        decimal incomeBalanceTotalToDate;
        decimal expenseBalanceTotalToDate;

        incomeBalanceTotalToDate = await unitOfWork.IncomeRepository.GetTotalToDateAsync(incomesCtgIds, houseHoldmembersIds, periodStart, userId, ct);
        expenseBalanceTotalToDate = await unitOfWork.ExpenseRepository.GetTotalToDateAsync(expensesCtgIds, houseHoldmembersIds, periodStart, userId, ct);

        var (incomeChart, expenseChart, balanceChart, incomePeriodSum, expensePeriodSum) = GetPeriodStatistics(
            model.SelectedYear,
            model.SelectedMonth,
            expenses, 
            incomes,
            incomeBalanceTotalToDate,
            expenseBalanceTotalToDate);

        logger.LogInformation("Multi (JS) statistics loaded for user {UserId}. Year: {Year}, Month: {Month}, Incomes: {IncomeCount}, Expenses: {ExpenseCount}.",
            userId,
            model.SelectedYear,
            model.SelectedMonth == 0 ? "whole year" : model.SelectedMonth.ToString(),
            incomes.Count,
            expenses.Count);

        return new StatisticsJsViewModel
        {
            IncomeTotal = incomeBalanceTotalToDate + incomePeriodSum,
            ExpenseTotal = expenseBalanceTotalToDate + expensePeriodSum,
            IncomeCategories = incomeCategoriesListItems,
            ExpenseCategories = expenseCategoriesListItems,
            HouseholdMembers = houseHoldmembersListItems,
            Months = months,
            Years = years,
            SelectedYear = model.SelectedYear,
            SelectedMonth = model.SelectedMonth,
            IncomeChart = incomeChart,
            ExpenseChart = expenseChart,
            BalanceChart = balanceChart
        };
    }

    //private helper methods -----------------------------------------------------------------------------------------

    private static (
        Dictionary<string, decimal>,
        Dictionary<string, decimal>,
        Dictionary<string, decimal>,
        decimal incomePeriodSum, 
        decimal expensePeriodSum
        ) GetPeriodStatistics(
        int year,
        int month,
        List<Expense> expenses,
        List<Income> incomes,
        decimal incomeBalanceTotal,
        decimal expenseBalanceTotal)
    {
        var incomeChart = new Dictionary<string, decimal>();
        var expenseChart = new Dictionary<string, decimal>();
        var balanceChart = new Dictionary<string, decimal>();
        decimal periodIncomeSum = 0;
        decimal periodExpenseSum = 0;

        if (month == 0)
        {
            decimal tempIncomeSum = 0;
            decimal tempExpenseSum = 0;

            for (int i = 1; i <= 12; i++)
            {
                incomeChart.Add(i.ToString(), incomes.Where(x => x.Date.Month == i).Sum(x => x.Amount));
                expenseChart.Add(i.ToString(), expenses.Where(x => x.Date.Month == i).Sum(x => x.Amount));

                tempIncomeSum += incomeChart[i.ToString()];
                tempExpenseSum += expenseChart[i.ToString()];

                periodIncomeSum += incomeChart[i.ToString()];
                periodExpenseSum += expenseChart[i.ToString()];

                var balance = (incomeBalanceTotal + tempIncomeSum) - (expenseBalanceTotal + tempExpenseSum);

                balanceChart.Add(i.ToString(), balance);
            }
        }
        else
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);

            decimal tempIncomeSum = 0;
            decimal tempExpenseSum = 0;

            for (int i = 1; i <= daysInMonth; i++)
            {
                incomeChart.Add(i.ToString(), incomes.Where(x => x.Date.Day == i).Sum(x => x.Amount));
                expenseChart.Add(i.ToString(), expenses.Where(x => x.Date.Day == i).Sum(x => x.Amount));

                tempIncomeSum += incomeChart[i.ToString()];
                tempExpenseSum += expenseChart[i.ToString()];

                periodIncomeSum += incomeChart[i.ToString()];
                periodExpenseSum += expenseChart[i.ToString()];

                var balance = (incomeBalanceTotal + tempIncomeSum) - (expenseBalanceTotal + tempExpenseSum);

                balanceChart.Add(i.ToString(), balance);
            }
        }

        return (incomeChart, expenseChart, balanceChart, periodIncomeSum, periodExpenseSum);
    }

    private async Task<(List<SelectListItem>, List<SelectListItem>, List<SelectListItem>)> GetSelectListData(string userId, CancellationToken ct)
    {
        var houseHoldmembers = await unitOfWork.HouseholdMemberRepository.GetAllActiveAsync(userId, ct);
        var houseHoldmembersListItems = houseHoldmembers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Nickname
        })
        .ToList();

        var incomeCategories = await unitOfWork.IncomeCategoryRepository.GetAllActiveAsync(userId, ct);
        var incomeCategoriesListItems = incomeCategories.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        })
        .ToList();

        var expenseCategories = await unitOfWork.ExpenseCategoryRepository.GetAllActiveAsync(userId, ct);
        var expenseCategoriesListItems = expenseCategories.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        })
        .ToList();

        houseHoldmembersListItems.Insert(0, new SelectListItem { Value = "0", Text = "Všichni" });
        incomeCategoriesListItems.Insert(0, new SelectListItem { Value = "0", Text = "Všechny"});
        expenseCategoriesListItems.Insert(0, new SelectListItem { Value = "0", Text = "Všechny" });

        return (houseHoldmembersListItems, incomeCategoriesListItems, expenseCategoriesListItems);
    }
}

