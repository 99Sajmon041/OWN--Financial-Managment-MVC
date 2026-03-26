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

        //IncomeCategories
        var selectedIncomeCategories = incomes.Select(x => new SelectListItem
        {
            Value = x.IncomeCategoryId.ToString(),
            Text = x.IncomeCategory.Name
        });

        var incomeCategories = OptionsBuilder.GetExpenseOrIncomeOptions(false);

        foreach (var incomeCtg in incomeCategories)
        {
            if (selectedIncomeCategories.Contains(incomeCtg))
                incomeCtg.Selected = true;
        }

        //ExpenseCategories
        var selectedExpenseCategories = expenses.Select(x => new SelectListItem
        {
            Value = x.ExpenseCategoryId.ToString(),
            Text = x.ExpenseCategory.Name
        });

        var expenseCategories = OptionsBuilder.GetExpenseOrIncomeOptions(true);

        foreach (var expenseCtg in expenseCategories)
        {
            if (selectedExpenseCategories.Contains(expenseCtg))
                expenseCtg.Selected = true;
        }

        //HouseHoldMembers - zjistit všechny housemembery dle user Id a aktivní !!!
        var houseHoldmembers = await unitOfWork.HouseholdMemberRepository.GetAllActiveAsync(userId, ct);
        var houseHoldmembersListItems = houseHoldmembers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Nickname
        });

        foreach (var houseHoldmember in houseHoldmembersListItems)
        {
            if (model.HouseholdMemberIds.Contains(int.Parse(houseHoldmember.Value)))
                houseHoldmember.Selected = true;
        }

        //Continue tomorrow

        logger.LogInformation("");

        return new StatisticsViewModel();
    }
}
