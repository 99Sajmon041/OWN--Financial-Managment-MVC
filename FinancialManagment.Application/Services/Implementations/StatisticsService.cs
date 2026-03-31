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
    public async Task<StatisticsViewModel> GetStatisticsAsync(StatisticsFilterModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var incomes = await unitOfWork.IncomeRepository.GetForStatisticsAsync(
            model.IncomeCategoryId,
            model.HouseholdMemberId, 
            model.SelectedYear, 
            model.SelectedMonth,
            userId,
            ct);

        var expenses = await unitOfWork.ExpenseRepository.GetForStatisticsAsync(
            model.ExpenseCategoryId,
            model.HouseholdMemberId,
            model.SelectedYear,
            model.SelectedMonth,
            userId,
            ct);

        var (houseHoldmembersListItems, incomeCategoriesListItems, expenseCategoriesListItems) = await GetSelectListData(userId, ct);

        foreach (var houseHoldmember in houseHoldmembersListItems)
        {
            if (model.HouseholdMemberId == int.Parse(houseHoldmember.Value))
            {
                houseHoldmember.Selected = true;
                break;
            }
        }

        foreach (var incomeCtg in incomeCategoriesListItems)
        {
            if (model.IncomeCategoryId == int.Parse(incomeCtg.Value))
            {
                incomeCtg.Selected = true;
                break;
            }
        }

        foreach (var expenseCtg in expenseCategoriesListItems)
        {
            if (model.ExpenseCategoryId == int.Parse(expenseCtg.Value))
            {
                expenseCtg.Selected = true;
                break;
            }
        }

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

        var incomesCtgIds = model.IncomeCategoryId != 0 ? new List<int> { model.IncomeCategoryId } : null;
        var expensesCtgIds = model.ExpenseCategoryId != 0 ? new List<int> { model.ExpenseCategoryId } : null;
        var houseHoldmembersIds = model.HouseholdMemberId != 0 ? new List<int> { model.HouseholdMemberId } : null;

        incomeBalanceTotalToDate = await unitOfWork.IncomeRepository.GetTotalToDateAsync(incomesCtgIds, houseHoldmembersIds, periodStart, userId, ct);
        expenseBalanceTotalToDate = await unitOfWork.ExpenseRepository.GetTotalToDateAsync(expensesCtgIds, houseHoldmembersIds, periodStart, userId, ct);

        var (incomeChart, expenseChart, balanceChart, incomePeriodSum, expensePeriodSum) = GetPeriodStatistics(
            model.SelectedYear,
            model.SelectedMonth,
            expenses, incomes,
            incomeBalanceTotalToDate,
            expenseBalanceTotalToDate);

        logger.LogInformation("Statistics loaded for user {UserId}. Year: {Year}, Month: {Month}, Incomes: {IncomeCount}, Expenses: {ExpenseCount}.",
            userId,
            model.SelectedYear,
            model.SelectedMonth == 0 ? "whole year" : model.SelectedMonth.ToString(),
            incomes.Count,
            expenses.Count);

        return new StatisticsViewModel
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

    public async Task<StatisticsViewModel> GetJsStatisticsAsync(StatisticsJsFilterModel model, CancellationToken ct)
    {
        throw new NotImplementedException();
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

