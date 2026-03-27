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

        var incomeTotal = incomes.Sum(x => x.Amount);
        var expenseTotal = expenses.Sum(x => x.Amount);

        var(houseHoldmembersListItems, incomeCategoriesListItems, expenseCategoriesListItems) = await GetSelectListData(userId, ct);

        foreach (var houseHoldmember in houseHoldmembersListItems)
        {
            if (model.HouseholdMemberId ==  int.Parse(houseHoldmember.Value))
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

        var (incomeChart, expenseChart, balanceChart) = GetPeriodStatistics(model.SelectedYear, model.SelectedMonth, expenses, incomes);

        logger.LogInformation("Statistics loaded for user {UserId}. Year: {Year}, Month: {Month}, Incomes: {IncomeCount}, Expenses: {ExpenseCount}.",
            userId,
            model.SelectedYear,
            model.SelectedMonth == 0 ? "whole year" : model.SelectedMonth.ToString(),
            incomes.Count,
            expenses.Count);

        return new StatisticsViewModel
        {
            IncomeTotal = incomeTotal,
            ExpenseTotal = expenseTotal,
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

    public async Task<StatisticsViewModel> GetJSStatisticsAsync(StatisticsJSFilterModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var incomes = await unitOfWork.IncomeRepository.GetForJSStatisticsAsync(
            model.IncomeCategoriesId,
            model.HouseholdMembersId,
            model.SelectedYear,
            model.SelectedMonth,
            userId,
            ct);

        var expenses = await unitOfWork.ExpenseRepository.GetForJSStatisticsAsync(
            model.ExpenseCategoriesId,
            model.HouseholdMembersId,
            model.SelectedYear,
            model.SelectedMonth,
            userId,
            ct);

        var incomeTotal = incomes.Sum(x => x.Amount);
        var expenseTotal = expenses.Sum(x => x.Amount);

        var (houseHoldmembersListItems, incomeCategoriesListItems, expenseCategoriesListItems) = await GetSelectListData(userId, ct);

        var selectedHouseholdMembersIds = model.HouseholdMembersId.ToHashSet();
        var incomeCategoriesId = model.IncomeCategoriesId.ToHashSet();
        var expenseCategoriesId = model.ExpenseCategoriesId.ToHashSet();

        foreach (var houseHoldmember in houseHoldmembersListItems)
        {
            if (selectedHouseholdMembersIds.Contains(int.Parse(houseHoldmember.Value)))
                houseHoldmember.Selected = true;
        }

        foreach (var incomeCtg in incomeCategoriesListItems)
        {
            if (incomeCategoriesId.Contains(int.Parse(incomeCtg.Value)))
                incomeCtg.Selected = true;
        }

        foreach (var expenseCtg in expenseCategoriesListItems)
        {
            if (expenseCategoriesId.Contains(int.Parse(expenseCtg.Value)))
                expenseCtg.Selected = true;
        }

        var months = OptionsBuilder.GetMonths(model.SelectedMonth);
        var years = OptionsBuilder.GetYears(model.SelectedYear);

        var (incomeChart, expenseChart, balanceChart) = GetPeriodStatistics(model.SelectedYear, model.SelectedMonth, expenses, incomes);

        logger.LogInformation("Statistics loaded for user {UserId}. Year: {Year}, Month: {Month}, Incomes: {IncomeCount}, Expenses: {ExpenseCount}.",
            userId,
            model.SelectedYear,
            model.SelectedMonth == 0 ? "whole year" : model.SelectedMonth.ToString(),
            incomes.Count,
            expenses.Count);

        return new StatisticsViewModel
        {
            IncomeTotal = incomeTotal,
            ExpenseTotal = expenseTotal,
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

    private static (Dictionary<string, decimal>, Dictionary<string, decimal>, Dictionary<string, decimal>) GetPeriodStatistics(
        int year,
        int month,
        List<Expense> expenses,
        List<Income> incomes)
    {
        var incomeChart = new Dictionary<string, decimal>();
        var expenseChart = new Dictionary<string, decimal>();
        var balanceChart = new Dictionary<string, decimal>();

        if (month == 0)
        {
            incomeChart = Enumerable.Range(1, 12)
                .ToDictionary(month => month.ToString(), month => incomes
                .Where(x => x.Date.Month == month).Sum(x => x.Amount));

            expenseChart = Enumerable.Range(1, 12)
                .ToDictionary(month => month.ToString(), month => expenses
                .Where(x => x.Date.Month == month).Sum(x => x.Amount));

            balanceChart = Enumerable.Range(1, 12)
                .ToDictionary(month => month.ToString(), month => incomeChart[month.ToString()] - expenseChart[month.ToString()]);
        }
        else
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);

            incomeChart = Enumerable.Range(1, daysInMonth)
                .ToDictionary(day => day.ToString(), day => incomes
                .Where(x => x.Date.Day == day).Sum(x => x.Amount));

            expenseChart = Enumerable.Range(1, daysInMonth)
                .ToDictionary(day => day.ToString(), day => expenses
                .Where(x => x.Date.Day == day).Sum(x => x.Amount));

            balanceChart = Enumerable.Range(1, daysInMonth)
                .ToDictionary(day => day.ToString(), day => incomeChart[day.ToString()] - expenseChart[day.ToString()]);
        }

        return (incomeChart, expenseChart, balanceChart);
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

        return (houseHoldmembersListItems, incomeCategoriesListItems, expenseCategoriesListItems);
    }
}