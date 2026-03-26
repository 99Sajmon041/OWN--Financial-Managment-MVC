using FinancialManagment.Application.FilterModels;
using FinancialManagment.Application.Models.Statistics;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
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
            model.IncomeCategoriesId,
            model.HouseholdMemberIds, 
            model.SelectedYear, 
            model.SelectedMonth,
            userId,
            ct);

        var expenses = await unitOfWork.ExpenseRepository.GetForStatisticsAsync(
            model.ExpenseCategoriesId,
            model.HouseholdMemberIds,
            model.SelectedYear,
            model.SelectedMonth,
            userId,
            ct);

        var incomeTotal = incomes.Sum(x => x.Amount);
        var expenseTotal = expenses.Sum(x => x.Amount);

        var houseHoldmembers = await unitOfWork.HouseholdMemberRepository.GetAllActiveAsync(userId, ct);
        var selectedHouseholdMembersIds = model.HouseholdMemberIds.ToHashSet();

        var houseHoldmembersListItems = houseHoldmembers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Nickname
        })
        .ToList();

        foreach (var houseHoldmember in houseHoldmembersListItems)
        {
            if (selectedHouseholdMembersIds.Contains(int.Parse(houseHoldmember.Value)))
                houseHoldmember.Selected = true;
        }

        var incomeCategories = await unitOfWork.IncomeCategoryRepository.GetAllActiveAsync(userId, ct);
        var incomeCategoriesId = model.IncomeCategoriesId.ToHashSet();

        var incomeCategoriesListItems = incomeCategories.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        })
        .ToList();

        foreach (var incomeCtg in incomeCategoriesListItems)
        {
            if (incomeCategoriesId.Contains(int.Parse(incomeCtg.Value)))
                incomeCtg.Selected = true;
        }

        var expenseCategories = await unitOfWork.ExpenseCategoryRepository.GetAllActiveAsync(userId, ct);
        var expenseCategoriesId = model.ExpenseCategoriesId.ToHashSet();

        var expenseCategoriesListItems = expenseCategories.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        })
        .ToList();

        foreach (var expenseCtg in expenseCategoriesListItems)
        {
            if (expenseCategoriesId.Contains(int.Parse(expenseCtg.Value)))
                expenseCtg.Selected = true;
        }

        var months = OptionsBuilder.GetMonths(model.SelectedMonth);
        var years = OptionsBuilder.GetYears(model.SelectedYear);

        var incomeChart = new Dictionary<string, decimal>();
        var expenseChart = new Dictionary<string, decimal>();
        var balanceChart = new Dictionary<string, decimal>();

        if (model.SelectedMonth == 0)
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
            var daysInMonth = DateTime.DaysInMonth(model.SelectedYear, model.SelectedMonth);
            
            incomeChart = Enumerable.Range(1, daysInMonth)
                .ToDictionary(day => day.ToString(), day => incomes
                .Where(x => x.Date.Day == day).Sum(x => x.Amount));

            expenseChart = Enumerable.Range(1, daysInMonth)
                .ToDictionary(day => day.ToString(), day => expenses
                .Where(x => x.Date.Day == day).Sum(x => x.Amount));

            balanceChart = Enumerable.Range(1, daysInMonth)
                .ToDictionary(day => day.ToString(), day => incomeChart[day.ToString()] - expenseChart[day.ToString()]);
        }

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
}